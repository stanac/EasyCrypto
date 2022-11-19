using System;
using Xunit;

namespace EasyCrypto.Tests
{
    public class PasswordHasherAndValidatorTests
    {
        [Fact]
        public void HashedPassword_ValidateSamePassword_ReturnsValid()
        {
            PasswordHasherAndValidator hasher = new PasswordHasherAndValidator();

            string password = Guid.NewGuid().ToString();

            string hash = hasher.HashPasswordToString(password);

            PasswordHashValidationResult valid = hasher.ValidatePassword(password, hash);
        }
    }
}
