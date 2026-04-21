using GameVoting.Data;
using GameVoting.Models.Entities;
using GameVoting.Repositories;
using GameVoting.Repositories.Interfaces;
using GameVoting.Services;
using GameVoting.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
}
else
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    var uri = new Uri(databaseUrl!);
    var userInfo = uri.UserInfo.Split(':');

    var builderNpgsql = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = uri.UserInfo.Split(':')[0],
        Password = uri.UserInfo.Split(':')[1],
        SslMode = Npgsql.SslMode.Require
    };

    var connectionString = builderNpgsql.ToString();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
});

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
builder.Services.AddScoped<ISiteSettingsRepository, SiteSettingsRepository>();

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IGameSessionService, GameSessionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("Provider: " + db.Database.ProviderName);
    Console.WriteLine("CanConnect: " + db.Database.CanConnect());

    var migrations = db.Database.GetMigrations().ToList();
    Console.WriteLine("Migrations count: " + migrations.Count);

    foreach (var m in migrations)
    {
        Console.WriteLine("Migration: " + m);
    }

    try
    {
        db.Database.Migrate();
        Console.WriteLine("Migrate executado com sucesso");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao migrar: " + ex);
        throw;
    }
    //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Game}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
