using GameVoting.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace GameVoting.UnitTests.Validation;

public class StoreUrlAttributeTests
{
    private readonly StoreUrlAttribute _attribute = new();

    private ValidationResult? Validate(string? value)
    {
        var context = new ValidationContext(new object());
        return _attribute.GetValidationResult(value, context);
    }

    [Fact]
    public void Validate_WhenUrlIsNull_ShouldBeValid()
    {
        var result = Validate(null);
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void Validate_WhenUrlIsEmpty_ShouldBeValid()
    {
        var result = Validate("");
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void Validate_WhenUrlIsFromAllowedDomain_ShouldBeValid()
    {
        var result = Validate("https://store.steampowered.com/app/1245620/ELDEN_RING/");
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void Validate_WhenUrlIsFromNotAllowedDomain_ShouldBeInvalid()
    {
        var result = Validate("https://malicious.com/fake-game");
        Assert.NotEqual(ValidationResult.Success, result);
    }

    [Fact]
    public void Validate_WhenUrlIsInvalid_ShouldBeInvalid()
    {
        var result = Validate("not-a-url");
        Assert.NotEqual(ValidationResult.Success, result);
    }
}
