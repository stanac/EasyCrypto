using EasyCrypto.Exceptions;
using EasyCrypto.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void UnalteredKeyCheckValueIsValidKeyLength32()
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

            Assert.False(exceptionWasThrown, "Validation of KCV has faild");
        }
    }
}
