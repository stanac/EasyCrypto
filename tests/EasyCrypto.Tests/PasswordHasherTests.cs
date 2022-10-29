using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EasyCrypto.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashedPasswordIsValid()
        {
            var ph = new PasswordHasher();
            string password = PasswordGenerator.GenerateStatic();
            string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
            bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
            Assert.True(isValid, "Password hash and validation failed");
        }
        
        [Fact]
        public void ChangedHashedPasswordIsNotValid()
        {
            var ph = new PasswordHasher();
            string password = PasswordGenerator.GenerateStatic();
            string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
            bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password + "!", hashAndSalt);
            Assert.False(isValid, "Password hash and validation failed for changed password");
        }

        [Fact]
        public void HashedPasswordIsValidHashSalt8()
        {
            var ph = new PasswordHasher(8);
            string password = PasswordGenerator.GenerateStatic();
            string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
            bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
            Assert.True(isValid, "Password hash and validation for 8 bytes failed");
        }
        
        [Fact]
        public void HashedPasswordIsValidHashSalt32()
        {
            var ph = new PasswordHasher(32);
            string password = PasswordGenerator.GenerateStatic();
            string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
            bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
            Assert.True(isValid, "Password hash and validation for 8 bytes failed");
        }
        
        [Fact]
        public void HashedPasswordIsValidHashSalt64()
        {
            var ph = new PasswordHasher(64);
            string password = PasswordGenerator.GenerateStatic();
            string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
            bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
            Assert.True(isValid, "Password hash and validation for 8 bytes failed");
        }

        //[Fact]
        //public void HashedPasswordIsValidHashSalt128()
        //{
        //    var ph = new PasswordHasher(128);
        //    string password = PasswordGenerator.GenerateStatic();
        //    string hashAndSalt = ph.HashPasswordAndGenerateEmbeddedSaltAsString(password);
        //    bool isValid = ph.ValidatePasswordWithEmbeddedSalt(password, hashAndSalt);
        //    Assert.True(isValid, "Password hash and validation for 8 bytes failed");
        //}

    }
}
