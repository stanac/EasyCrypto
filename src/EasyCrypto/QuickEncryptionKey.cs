namespace EasyCrypto;

/// <summary>
/// Key to be used with <see cref="QuickEncryption"/>
/// </summary>
public class QuickEncryptionKey
{
    internal string Value { get; }
    internal byte[] Key { get; }

    private const int KeyLength = 32;

    private QuickEncryptionKey(string value)
    {
        Value = value;
        SHA256 sha = SHA256.Create();
        Key = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Creates new randomly generated key
    /// </summary>
    /// <returns></returns>
    public static QuickEncryptionKey CreateNew()
    {
        string token = TokenGenerator.Default.GenerateToken(KeyLength);
        return new QuickEncryptionKey(token);
    }

    /// <summary>
    /// Parses key from string
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <returns><see cref="QuickEncryptionKey"/></returns>
    /// <exception cref="ArgumentException">Throw in case of not valid argument</exception>
    public static QuickEncryptionKey Parse(string value)
    {
        if (value.Length != KeyLength)
        {
            throw new ArgumentException("Not valid value");
        }

        return new QuickEncryptionKey(value);
    }

    /// <summary>
    /// Returns key represented as string
    /// </summary>
    /// <returns>String value of the key</returns>
    public override string ToString() => Value;
}