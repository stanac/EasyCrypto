using EasyCrypto.Exceptions;
using EasyCrypto.Validation;
using Xunit;

namespace EasyCrypto.Tests
{
    public class KeyCheckValueValidatorTests
    {
        [Fact]
        public void KeyCheckValidIs19BytesLong()
        {
            byte[] key = CryptoRandom.NextBytesStatic(32);
            byte[] kcv = KeyCheckValueValidator.GenerateKeyCheckValue(key);
            Assert.True(kcv.Length == 19, "KCV is not 19 bytes long");
        }

        [Fact]
        public void UnalteredKeyCheckValueIsValid()
        {
            byte[] key = CryptoRandom.NextBytesStatic(32);
            byte[] kcv = KeyCheckValueValidator.GenerateKeyCheckValue(key);
            
            bool exceptionWasThrown = false;
            try
            {
                KeyCheckValueValidator.ValidateKeyCheckValue(key, kcv);
            }
            catch (KeyCheckValueValidationException)
            {
                exceptionWasThrown = true;
            }

            Assert.False(exceptionWasThrown, "Validation of KCV has failed");
        }

        [Fact]
        public void AlteredKeyCheckValueIsNotValid()
        {
            byte[] key = CryptoRandom.NextBytesStatic(32);
            byte[] kcv = KeyCheckValueValidator.GenerateKeyCheckValue(key);
            if (key[0] > 120) key[0]--;
            else key[0]++;

            bool exceptionWasThrown = false;
            try
            {
                KeyCheckValueValidator.ValidateKeyCheckValue(key, kcv);
            }
            catch (KeyCheckValueValidationException)
            {
                exceptionWasThrown = true;
            }

            Assert.True(exceptionWasThrown, "Validation of KCV should have failed, but it didn't");
        }
    }
}
