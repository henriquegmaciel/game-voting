using System.Security.Claims;
using GameVoting.Models.Entities;
using GameVoting.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameVoting.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        ApplicationUser user = new()
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName,
            RegisteredAt = DateTime.UtcNow
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("Index", "Game");
        }

        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
            return RedirectToAction("Index", "Game");

        ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Game");
    }

    [HttpGet]
    public IActionResult SteamLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action("SteamCallback", "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Steam", redirectUrl);
        return Challenge(properties, "Steam");
    }

    [HttpGet]
    public async Task<IActionResult> SteamCallback(string? returnUrl = null)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return RedirectToAction("Login");

        var steamId = info.ProviderKey.Split('/').Last();
        var steamName = info.Principal.Identity?.Name ?? steamId;

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: true);

        if (result.Succeeded)
        {
            var existingUser = await _userManager.FindByNameAsync(steamId);
            if (existingUser is not null && existingUser.DisplayName != steamName)
            {
                UpdateUserDisplayName(existingUser, steamName);
                UpdateDisplayNameClaim(existingUser, steamName);
            }
            return LocalRedirect(returnUrl ?? "/");
        }

        var user = new ApplicationUser
        {
            UserName = steamId,
            DisplayName = steamName,
            RegisteredAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(user, info);
            await _userManager.AddClaimAsync(user, new Claim("DisplayName", steamName));
            await _signInManager.SignInAsync(user, isPersistent: true);

            return LocalRedirect(returnUrl ?? "/");
        }

        return RedirectToAction("Login");
    }

    private async void UpdateUserDisplayName(ApplicationUser user, string newName)
    {
        user.DisplayName = newName;
        await _userManager.UpdateAsync(user);
    }

    private async void UpdateDisplayNameClaim(ApplicationUser user, string newName)
    {
        var oldClaim = await GetDisplayNameClaim(user);
        if (oldClaim is not null)
            await _userManager.ReplaceClaimAsync(user, oldClaim, new Claim("DisplayName", newName));
        else
            await _userManager.AddClaimAsync(user, new Claim("DisplayName", newName));
    }

    private async Task<Claim?> GetDisplayNameClaim(ApplicationUser user)
    {
        return (await _userManager.GetClaimsAsync(user))
                    .FirstOrDefault(c => c.Type == "DisplayName");
    }
}
