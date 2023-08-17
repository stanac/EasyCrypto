namespace EasyCrypto;

/// <summary>
/// Allowed sizes of RSA keys
/// </summary>
public enum RsaKeySizes
{
    /// <summary>
    /// 2048 bits
    /// </summary>
    Rsa2048 = 0x4,

    /// <summary>
    /// 4096 bits
    /// </summary>
    Rsa4096 = 0x5,

    /// <summary>
    /// 8192 bits
    /// </summary>
    Rsa8192 = 0x6,

    /// <summary>
    /// 16384 bits
    /// </summary>
    Rsa16384 = 0x7
}

/// <summary>
/// Extension methods for <see cref="RsaKeySizes"/>
/// </summary>
public static class RsaKeySizesExtensions
{
    /// <summary>
    /// Maps <see cref="RsaKeySizes"/> to integers
    /// </summary>
    /// <param name="size">Enum value</param>
    /// <returns>Integer value</returns>
    public static int GetIntegerKeySize(this RsaKeySizes size)
    {
        switch (size)
        {
            case RsaKeySizes.Rsa2048:
                return 2048;

            case RsaKeySizes.Rsa4096:
                return 4096;

            case RsaKeySizes.Rsa8192:
                return 8192;

            default:
                return 16384;
        }
    }
}