using System;
using Xunit;

namespace EasyCrypto.Tests
{
    public class RsaEncryptionTests
    {
        [Fact]
        public void GenerteKey_GeneratesKeyPair()
        {
            var keys = RsaEncryption.GenerateKeyPairs();

            Assert.NotNull(keys);
            Assert.False(string.IsNullOrWhiteSpace(keys.PrivateKey?.Key));
            Assert.False(string.IsNullOrWhiteSpace(keys.PublicKey?.Key));

            Assert.True(keys.PrivateKey.Key.Length > keys.PublicKey.Key.Length);
        }

        [Fact]
        public void ByteArray_EncryptDecrypt_GivesEqualByteArray()
        {
            var keys = RsaEncryption.GenerateKeyPairs();

            Guid plainText = Guid.NewGuid();
            byte[] encrypted = RsaEncryption.Encrypt(plainText.ToByteArray(), keys.PublicKey);
            byte[] decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);
            Guid result = new Guid(decrypted);

            Assert.Equal(plainText, result);
        }

        [Fact]
        public void String_EncryptDecrypt_GivesEqualString()
        {
            var keys = RsaEncryption.GenerateKeyPairs();

            var plainText = Guid.NewGuid().ToString();
            string encrypted = RsaEncryption.Encrypt(plainText, keys.PublicKey);
            string decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);
            
            Assert.Equal(plainText, decrypted);
        }

        [Fact]
        public void String_Encrypt2048Decrypt_GivesEqualString()
        {
            var keys = RsaEncryption.GenerateKeyPairs(RsaKeySizes.Rsa2048);

            var plainText = Guid.NewGuid().ToString();
            string encrypted = RsaEncryption.Encrypt(plainText, keys.PublicKey);
            string decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);

            Assert.Equal(plainText, decrypted);
        }

        [Fact]
        public void String_Encrypt4096Decrypt_GivesEqualString()
        {
            var keys = RsaEncryption.GenerateKeyPairs(RsaKeySizes.Rsa4096);

            var plainText = Guid.NewGuid().ToString();
            string encrypted = RsaEncryption.Encrypt(plainText, keys.PublicKey);
            string decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);

            Assert.Equal(plainText, decrypted);
        }

        // super slow // [Fact]
        // super slow // public void String_Encrypt8192Decrypt_GivesEqualString()
        // super slow // {
        // super slow //     var keys = RsaEncryption.GenerateKeyPairs(RsaKeySizes.Rsa8192);
        // super slow // 
        // super slow //     var plainText = Guid.NewGuid().ToString();
        // super slow //     string encrypted = RsaEncryption.Encrypt(plainText, keys.PublicKey);
        // super slow //     string decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);
        // super slow // 
        // super slow //     Assert.Equal(plainText, decrypted);
        // super slow // }
        // super slow // 
        // super slow // [Fact]
        // super slow // public void String_Encrypt16384Decrypt_GivesEqualString()
        // super slow // {
        // super slow //     var keys = RsaEncryption.GenerateKeyPairs(RsaKeySizes.Rsa16384);
        // super slow // 
        // super slow //     var plainText = Guid.NewGuid().ToString();
        // super slow //     string encrypted = RsaEncryption.Encrypt(plainText, keys.PublicKey);
        // super slow //     string decrypted = RsaEncryption.Decrypt(encrypted, keys.PrivateKey);
        // super slow // 
        // super slow //     Assert.Equal(plainText, decrypted);
        // super slow // }

    }
}
