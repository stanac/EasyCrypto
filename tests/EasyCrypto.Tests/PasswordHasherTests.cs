using Xunit;
#pragma warning disable CS0618

namespace EasyCrypto.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashedPasswordIsValid()
    {
        PasswordHasher ph = new PasswordHasher();
        string password = PasswordGenerator.Default.Generate();
        string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
        bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
        Assert.True(isValid, "Password hash and validation failed");
    }
        
    [Fact]
    public void ChangedHashedPasswordIsNotValid()
    {
        PasswordHasher ph = new PasswordHasher();
        string password = PasswordGenerator.Default.Generate();
        string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
        bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password + "!", hashAndSalt);
        Assert.False(isValid, "Password hash and validation failed for changed password");
    }

    [Fact]
    public void HashedPasswordIsValidHashSalt8()
    {
        PasswordHasher ph = new PasswordHasher(8);
        string password = PasswordGenerator.Default.Generate();
        string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
        bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
        Assert.True(isValid, "Password hash and validation for 8 bytes failed");
    }
    
    [Fact]
    public void HashedPasswordIsValidHashSalt32()
    {
        PasswordHasher ph = new PasswordHasher(32);
        string password = PasswordGenerator.Default.Generate();
        string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
        bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
        Assert.True(isValid, "Password hash and validation for 8 bytes failed");
    }
    
    [Fact]
    public void HashedPasswordIsValidHashSalt64()
    {
        PasswordHasher ph = new PasswordHasher(64);
        string password = PasswordGenerator.Default.Generate();
        string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
        bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
        Assert.True(isValid, "Password hash and validation for 8 bytes failed");
    }
}