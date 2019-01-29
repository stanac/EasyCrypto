using EasyCrypto.Exceptions;
using EasyCrypto.Validation;
using System.IO;
using Xunit;

namespace EasyCrypto.Tests
{
    public class MessageAuthenticationCodeValidatorTests
    {
        [Fact]
        public void MacIs48BytesLong()
        {
            byte[] data = CryptoRandom.NextBytesStatic(1227);
            byte[] key = CryptoRandom.NextBytesStatic(32);
            using (Stream tempStream = new MemoryStream())
            {
                tempStream.Write(data, 0, data.Length);
                byte[] mac = MessageAuthenticationCodeValidator.CalculateMessageAuthenticationCode(key, tempStream);
                Assert.Equal(48, mac.Length);
            }
        }

        [Fact]
        public void MacCanBeValidated()
        {
            byte[] data = CryptoRandom.NextBytesStatic(1227);
            byte[] key = CryptoRandom.NextBytesStatic(32);
            using (Stream tempStream = new MemoryStream())
            {
                tempStream.Write(data, 0, data.Length);
                bool error = false;
                byte[] mac = MessageAuthenticationCodeValidator.CalculateMessageAuthenticationCode(key, tempStream);
                try
                {
                    MessageAuthenticationCodeValidator.ValidateMessageAuthenticationCode(key, mac, tempStream);
                }
                catch (DataIntegrityValidationException)
                {
                    error = true;
                }
                Assert.False(error);
            }
        }
    }
}
