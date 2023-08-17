using System;
using FluentAssertions;
using Xunit;

namespace EasyCrypto.Tests;

public class PasswordHasherAndValidatorTests
{
    [Fact]
    public void HashedPassword_ValidateSamePassword_ReturnsValid()
    {
        PasswordHasherAndValidator hasher = new PasswordHasherAndValidator();
        string password = Guid.NewGuid().ToString();
        string hash = hasher.HashPasswordToString(password);
        PasswordHashValidationResult valid = hasher.ValidatePassword(password, hash);
        valid.Should().Be(PasswordHashValidationResult.Valid);
    }

    [Fact]
    public void HashedPassword_ValidateWrongPassword_ReturnsFalse()
    {
        PasswordHasherAndValidator hasher = new PasswordHasherAndValidator();
        string password = Guid.NewGuid().ToString();
        string hash = hasher.HashPasswordToString(password);
        password = Guid.NewGuid().ToString();
        PasswordHashValidationResult valid = hasher.ValidatePassword(password, hash);
        valid.Should().Be(PasswordHashValidationResult.NotValid);
    }

    [Fact]
    public void HashedPassword_ValidateOldSettings_ReturnsShouldRehash()
    {
        PasswordHasherAndValidator hasher = new PasswordHasherAndValidator(29_000);
        string password = Guid.NewGuid().ToString();
        string hash = hasher.HashPasswordToString(password);

        hasher = new PasswordHasherAndValidator(29_001);
        PasswordHashValidationResult valid = hasher.ValidatePassword(password, hash);
        valid.Should().Be(PasswordHashValidationResult.ValidShouldRehash);
    }
}