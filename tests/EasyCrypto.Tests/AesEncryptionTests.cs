using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace EasyCrypto.Tests;

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
                string password = PasswordGenerator.Default.Generate();
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(4 * 1025, 8 * 1025));

                byte[] encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                byte[] decrypted = AesEncryption.DecryptWithPassword(encrypted, password);

                string plainString = Convert.ToBase64String(plainText);
                string decryptedString = Convert.ToBase64String(decrypted);

                Assert.Equal(plainString, decryptedString);
            }
        }
    }

    [Fact]
    public void EncryptedWithEmbeddedIvCanBeDecrypted()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(4 * 1025, 8 * 1025));

                byte[] encrypted = AesEncryption.EncryptAndEmbedIv(plainText, key);
                byte[] decrypted = AesEncryption.DecryptWithEmbeddedIv(encrypted, key);

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
    public async Task EncryptedWithPasswordAsyncCanBeDecrypted()
    {
        string password = Guid.NewGuid().ToString();

        string tempPlainPath = Path.GetTempFileName();
        string tempEncPath = Path.GetTempFileName();
        string tempDecPath = Path.GetTempFileName();

        byte[] plain = System.Text.Encoding.ASCII.GetBytes(Guid.NewGuid().ToString());
        using (Stream plainStream = new FileStream(tempPlainPath, FileMode.Create))
        {
            plainStream.Write(plain, 0, plain.Length);
            plainStream.Flush();
        }

        using (Stream plainStream = new FileStream(tempPlainPath, FileMode.Open))
        using (Stream encryptedStream = new FileStream(tempEncPath, FileMode.Create))
        {
            await AesEncryption.EncryptWithPasswordAsync(plainStream, password, encryptedStream);
            encryptedStream.Flush();
        }

        using (Stream encryptedStream = new FileStream(tempEncPath, FileMode.Open))
        using (Stream decryptedStream = new FileStream(tempDecPath, FileMode.Create))
        {
            await AesEncryption.DecryptWithPasswordAsync(encryptedStream, password, decryptedStream);
            decryptedStream.Flush();
        }

        string text1 = File.ReadAllText(tempPlainPath);
        string text2 = File.ReadAllText(tempDecPath);

        File.Delete(tempDecPath);
        File.Delete(tempEncPath);
        File.Delete(tempPlainPath);

        Assert.Equal(text1, text2);
    }

    private void TestEncryptDecrypt(uint keySize)
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(keySize);
                byte[] iv = cr.NextBytes(16);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(1, 2455));

                byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                byte[] decrypted = AesEncryption.Decrypt(encrypted, key, iv);

                string plainString = Convert.ToBase64String(plainText);
                string decryptedString = Convert.ToBase64String(decrypted);

                Assert.Equal(plainString, decryptedString);
            }
        }
    }
}