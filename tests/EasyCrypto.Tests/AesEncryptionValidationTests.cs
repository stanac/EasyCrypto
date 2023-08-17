using System;
using Xunit;
using static EasyCrypto.Exceptions.DataFormatValidationException;

namespace EasyCrypto.Tests;

public class AesEncryptionValidationTests
{
    #region passing both validations

    [Fact]
    public void EncryptedDataValidationIsValid()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] iv = cr.NextBytes(16);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                ValidationResult result = AesEncryption.ValidateEncryptedData(encrypted, key, iv);
                Assert.True(result.IsValid);
            }
        }
    }

    [Fact]
    public void EncryptedDataWithEmbeddedIvValidationIsValid()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.EncryptAndEmbedIv(plainText, key);
                ValidationResult result = AesEncryption.ValidateEncryptedDataWithEmbeddedIv(encrypted, key);
                Assert.True(result.IsValid);
            }
        }
    }

    [Fact]
    public void EncryptedDataWithPasswordIsValid()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                string plainText = Guid.NewGuid().ToString();
                string password = PasswordGenerator.Default.Generate();

                string encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                ValidationResult result = AesEncryption.ValidateEncryptedDataWithPassword(encrypted, password);
                Assert.True(result.IsValid);
            }
        }
    }

    #endregion

    #region KCV validation failing

    [Fact]
    public void EncryptedDataValidationFailsKcvOnChangedKey()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] iv = cr.NextBytes(16);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                AlterData(key, 0); // change key

                ValidationResult result = AesEncryption.ValidateEncryptedData(encrypted, key, iv);
                Assert.False(result.IsValid);
                Assert.False(result.KeyIsValid);
                Assert.Equal(DataValidationErrors.KeyCheckValueValidationError, result.ErrorType.Value);
            }
        }
    }

    [Fact]
    public void EncryptedDataWithEmbededIvFailsKcvWhenKeyIsAltered()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.EncryptAndEmbedIv(plainText, key);
                AlterData(key, 0);

                ValidationResult result = AesEncryption.ValidateEncryptedDataWithEmbeddedIv(encrypted, key);
                Assert.False(result.IsValid);
                Assert.False(result.KeyIsValid);
                Assert.Equal(DataValidationErrors.KeyCheckValueValidationError, result.ErrorType.Value);
            }
        }
    }

    [Fact]
    public void EncryptedDataWithAlteredPasswordFailsKcvValidation()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                string plainText = Guid.NewGuid().ToString();
                string password = PasswordGenerator.Default.Generate();

                string encrypted = AesEncryption.EncryptWithPassword(plainText, password);
                password = password.ToUpper();

                ValidationResult result = AesEncryption.ValidateEncryptedDataWithPassword(encrypted, password);
                Assert.False(result.IsValid);
                Assert.False(result.KeyIsValid);
                Assert.Equal(DataValidationErrors.KeyCheckValueValidationError, result.ErrorType.Value);
            }
        }
    }

    [Fact]
    public void EncryptedDataWithAlteredKcvFailsKcvValidation()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] iv = cr.NextBytes(16);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                AlterData(encrypted, 57); // change KCV

                ValidationResult result = AesEncryption.ValidateEncryptedData(encrypted, key, iv);
                Assert.False(result.IsValid);
                Assert.False(result.KeyIsValid);
                Assert.Equal(DataValidationErrors.KeyCheckValueValidationError, result.ErrorType.Value);
            }
        }
    }

    #endregion

    #region MAC validation failing

    [Fact]
    public void EncryptedAlteredDataWillFailMacValidation()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] iv = cr.NextBytes(16);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                AlterData(encrypted, encrypted.Length - 1); // alter encrypted data

                ValidationResult result = AesEncryption.ValidateEncryptedData(encrypted, key, iv);
                Assert.False(result.IsValid);
                Assert.False(result.DataIntegrityIsValid);
                Assert.Equal(DataValidationErrors.DataIntegrityValidationError, result.ErrorType.Value);
            }
        }
    }

    [Fact]
    public void EncryptedDataWithAlteredMacWillFailMacValidation()
    {
        using (var cr = new CryptoRandom())
        {
            for (int i = 0; i < 5; i++)
            {
                byte[] key = cr.NextBytes(32);
                byte[] iv = cr.NextBytes(16);
                byte[] plainText = cr.NextBytes((uint) cr.NextInt(14, 98));

                byte[] encrypted = AesEncryption.Encrypt(plainText, key, iv);
                AlterData(encrypted, 76); // alter MAC

                ValidationResult result = AesEncryption.ValidateEncryptedData(encrypted, key, iv);
                Assert.False(result.IsValid);
                Assert.False(result.DataIntegrityIsValid);
                Assert.Equal(DataValidationErrors.DataIntegrityValidationError, result.ErrorType.Value);
            }
        }
    }

    #endregion

    private void AlterData(byte[] data, int index)
    {
        if (data[index] > 120) data[index]--;
        else data[index]++;
    }
}