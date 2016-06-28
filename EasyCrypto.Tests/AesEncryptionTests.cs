using System;
using Xunit;

namespace EasyCrypto.Tests
{
    public class AesEncryptionTests
    {
        [Fact]
        public void EncryptedStringCanBeDecrypted()
        {
            var pg = new PasswordGenerator();
            for (int i = 0; i < 5; i++)
            {
                string plainText = pg.Generate();
                string password = pg.Generate();

                string encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                string decrypted = AesEncryption.DecryptWithPassword(encrypted, password);

                Assert.Equal(plainText, decrypted);
            }
        }

        [Fact]
        public void EncryptedWithPasswordCanBeDecrypted()
        {
            using (var cr = new CryptoRandom())
            {
                for (int i = 0; i < 5; i++)
                {
                    string password = PasswordGenerator.GenerateStatic();
                    byte[] plainText = cr.NextBytes((uint)cr.NextInt(4 * 1025, 8 * 1025));

                    byte[] encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                    byte[] decrypted = AesEncryption.DecryptWithPassword(encrypted, password);

                    string plainString = Convert.ToBase64String(plainText);
                    string decryptedString = Convert.ToBase64String(decrypted);

                    Assert.Equal(plainString, decryptedString);
                }
            }
        }

        [Fact]
        public void EncryptedWithEmbededIvCanBeDecrypted()
        {
            using (var cr = new CryptoRandom())
            {
                for (int i = 0; i < 5; i++)
                {
                    byte[] key = cr.NextBytes(32);
                    byte[] plainText = cr.NextBytes((uint)cr.NextInt(4 * 1025, 8 * 1025));

                    byte[] encrypted = AesEncryption.EncryptAndEmbedIv(plainText, key);
                    byte[] decrypted = AesEncryption.DecryptWithEmbededIv(encrypted, key);

                    string plainString = Convert.ToBase64String(plainText);
                    string decryptedString = Convert.ToBase64String(decrypted);

                    Assert.Equal(plainString, decryptedString);
                }
            }
        }

        [Fact]
        public void EncryptedDataCanBeDecryptedWithKey32()
            => TestEncryptDecrypt(32);

        [Fact]
        public void EncryptedDataCanBeDecryptedWithKey16()
            => TestEncryptDecrypt(16);

        [Fact]
        public void EncryptedDataCanBeDecryptedWithKey24()
            => TestEncryptDecrypt(24);

        //[Fact]
        //public void EncryptingWithSameParametersWillResultInSameOutput()
        //{
        //    byte[] data = Guid.NewGuid().ToByteArray();
        //    byte[] iv = CryptoRandom.NextBytesStatic(16);
        //    byte[] key = CryptoRandom.NextBytesStatic(16);

        //    byte[] encrypted1 = AesEncryption.Encrypt(data, key, iv);
        //    byte[] encrypted2 = AesEncryption.Encrypt(data, key, iv);

        //    bool areEqual = DataTools.CompareByteArrays(encrypted1, encrypted2);
        //    Assert.True(areEqual);
        //}

        private void TestEncryptDecrypt(uint keySize)
        {
            using (var cr = new CryptoRandom())
            {
                for (int i = 0; i < 5; i++)
                {
                    byte[] key = cr.NextBytes(keySize);
                    byte[] iv = cr.NextBytes(16);
                    byte[] plainText = cr.NextBytes((uint)cr.NextInt(1, 2455));

                    byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                    byte[] decrypted = AesEncryption.Decrypt(encrypted, key, iv);

                    string plainString = Convert.ToBase64String(plainText);
                    string decryptedString = Convert.ToBase64String(decrypted);

                    Assert.Equal(plainString, decryptedString);
                }
            }
        }
    }
}
