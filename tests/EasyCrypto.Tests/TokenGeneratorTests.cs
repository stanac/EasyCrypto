using System.Linq;
using Xunit;

namespace EasyCrypto.Tests;

public class TokenGeneratorTests
{
    [Fact]
    public void GenerateToken_DefinedChars_DoesNotContainOtherChars()
    {
        var tokenGen = new TokenGenerator("1234567890");
        var token = tokenGen.GenerateToken(260);

        int numberOfNonNumericChars = token.Count(c => !char.IsNumber(c));

        Assert.Equal(0, numberOfNonNumericChars);
    }

    [Fact]
    public void GenerateToken_GeneratesTokenFromDifferentChars()
    {
        var tokenGen = new TokenGenerator();
        var token = tokenGen.GenerateToken(1000);

        int numberOfUniqueChars = token.Distinct().Count();

        Assert.True(numberOfUniqueChars > 20); // we cannot actually test this in correct way because it's random
    }

    [Fact]
    public void ValidHashedToken_Validate_ReturnsTrue()
    {
        var tokenGen = new TokenGenerator();
        string token = tokenGen.GenerateToken(30);
        string hash = tokenGen.HashToken(token);
        bool isValid = tokenGen.ValidateTokenHash(token, hash);
        Assert.True(isValid);
    }

    [Fact]
    public void NotValidHashedToken_Validate_ReturnsFalse()
    {
        var tokenGen = new TokenGenerator();
        string token = tokenGen.GenerateToken(30);
        string hash = tokenGen.HashToken(token + "-");
        bool isValid = tokenGen.ValidateTokenHash(token, hash);
        Assert.False(isValid);
    }

    [Fact]
    public void QuickHash_Verify_ReturnsTrue()
    {
        var tokenGen = new TokenGenerator();
        string token = tokenGen.GenerateToken(30);
        string hash = tokenGen.HashToken(token, true);

        bool isValid = tokenGen.ValidateTokenHash(token, hash);
        Assert.True(isValid);
    }
}