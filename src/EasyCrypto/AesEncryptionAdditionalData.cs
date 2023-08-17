using EasyCrypto.Internal;

namespace EasyCrypto;

/// <summary>
/// Class used for embedding and reading additional data from already encrypted data.
/// Additional data is not protected, but it isn't added in plain text either.
/// </summary>
public static class AesEncryptionAdditionalData
{
    /// <summary>
    /// Adds additional data to encrypted data. Additional data is not protected, but it isn't added in plain text either.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to which to add additional data.</param>
    /// <param name="additionalData">The additional data to add. Dictionary items where key or value is null or empty will be ignored/</param>
    /// <returns>Encrypted data with added additional data</returns>
    public static string AddAdditionalData(string encryptedData, Dictionary<string, string> additionalData)
        => Convert.ToBase64String(AddAdditionalData(Convert.FromBase64String(encryptedData), additionalData));

    /// <summary>
    /// Adds additional data to encrypted data. Additional data is not protected, but it isn't added in plain text either.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to which to add additional data.</param>
    /// <param name="additionalData">The additional data to add. Dictionary items where key or value is null or empty will be ignored/</param>
    /// <returns>Encrypted data with added additional data</returns>
    public static byte[] AddAdditionalData(byte[] encryptedData, Dictionary<string, string> additionalData)
        => AesEncryption.HandleByteToStream(encryptedData, (inStream, outStream) => AddAdditionalData(inStream, additionalData, outStream));

    /// <summary>
    /// Adds additional data to encrypted data. Additional data is not protected, but it isn't added in plain text either.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to which to add additional data.</param>
    /// <param name="additionalData">The additional data to add. Dictionary items where key or value is null or empty will be ignored/</param>
    /// <param name="destination">Stream to which to write encrypted data with added additional data</param>
    public static void AddAdditionalData(Stream encryptedData, Dictionary<string, string> additionalData, Stream destination)
    {
        if (additionalData == null) throw new ArgumentNullException(nameof(additionalData));

        byte[] dataBytes = new AdditionalData(additionalData).GetBytes();
        CryptoContainer.WriteAdditionalData(encryptedData, dataBytes, destination);
    }

    /// <summary>
    /// Reads the additional data from encrypted data if present.
    /// </summary>
    /// <param name="encryptedData">The encrypted data.</param>
    /// <returns>Additional data</returns>
    public static Dictionary<string, string> ReadAdditionalData(string encryptedData)
        => ReadAdditionalData(Convert.FromBase64String(encryptedData));

    /// <summary>
    /// Reads the additional data from encrypted data if present.
    /// </summary>
    /// <param name="encryptedData">The encrypted data.</param>
    /// <returns>Additional data</returns>
    public static Dictionary<string, string> ReadAdditionalData(byte[] encryptedData)
    {
        using (Stream temp = new MemoryStream())
        {
            temp.Write(encryptedData, 0, encryptedData.Length);
            temp.Position = 0;
            return ReadAdditionalData(temp);
        }
    }

    /// <summary>
    /// Reads the additional data from encrypted data if present.
    /// </summary>
    /// <param name="encryptedData">The encrypted data.</param>
    /// <returns>Additional data</returns>
    public static Dictionary<string, string> ReadAdditionalData(Stream encryptedData)
    {
        byte[] data = CryptoContainer.ReadAdditionalData(encryptedData);
        return AdditionalData.LoadFromBytes(data).Data;
    }
}