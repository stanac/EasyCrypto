namespace EasyCrypto;

/// <summary>
/// Faster AES-256 to be used only with generated keys, not passwords, use <see cref="QuickEncryptionKey"/>
/// </summary>
public class QuickEncryption
{
    private readonly QuickEncryptionKey _key;

    public QuickEncryption(QuickEncryptionKey key)
    {
        _key = key;
    }

    public byte[] Encrypt(byte[] plainTextData) => Encrypt(plainTextData, _key);

    public string Encrypt(string plainText) => Encrypt(plainText, _key);

    public byte[] Decrypt(byte[] data) => Decrypt(data, _key);
    
    public string Decrypt(string encryptedData) => Decrypt(encryptedData, _key);

    public static string Encrypt(string plainText, QuickEncryptionKey key)
    {
        byte[] value = Encrypt(Encoding.UTF8.GetBytes(plainText), key);
        return Convert.ToBase64String(value);
    }

    public static string Decrypt(string encryptedData, QuickEncryptionKey key)
    {
        byte[] data = Decrypt(Convert.FromBase64String(encryptedData), key);
        return Encoding.UTF8.GetString(data);
    }

    public static byte[] Encrypt(byte[] plainTextData, QuickEncryptionKey key)
    {
        return AesEncryption.EncryptAndEmbedIv(plainTextData, key.Key);
    }

    public static byte[] Decrypt(byte[] data, QuickEncryptionKey key)
    {
        return AesEncryption.DecryptWithEmbeddedIv(data, key.Key);
    }
}