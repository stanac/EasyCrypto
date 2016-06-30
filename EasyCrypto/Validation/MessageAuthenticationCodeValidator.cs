using System.IO;
using System.Security.Cryptography;

namespace EasyCrypto.Validation
{
    /// <summary>
    /// Validates MAC
    /// </summary>
    public static class MessageAuthenticationCodeValidator
    {
        /// <summary>
        /// Calculates the message authentication code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="startIndex">Start index from which to calculate MAC, default 0W.</param>
        /// <returns></returns>
        public static byte[] CalculateMessageAuthenticationCode(byte[] key, Stream encryptedData, long startIndex = 0)
        {
            HMACSHA384 hmac = new HMACSHA384(key);
            long originalPosition = encryptedData.Position;
            encryptedData.Position = startIndex;
            byte[] mac = hmac.ComputeHash(encryptedData);
            encryptedData.Position = originalPosition;
            return mac;
        }

        /// <summary>
        /// Validates the message authentication code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="originalMac">The original mac.</param>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="EasyCrypto.Exceptions.DataIntegrityValidationException">Validation of Message Authentication Code (MAC) has failed.
        /// Most likely reason for this exception is that encrypted data was modified.</exception>
        public static void ValidateMessageAuthenticationCode(byte[] key, byte[] originalMac, Stream encryptedData, long startIndex = 0)
        {
            if (!ValidateMessageAuthenticationCodeInternal(key, originalMac, encryptedData, startIndex))
            {
                throw new Exceptions.DataIntegrityValidationException("Validation of Message Authentication Code (MAC) has failed. " +
                    "Most likely reason for this exception is that encrypted data was modified.");
            }
        }

        /// <summary>
        /// For internal use
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="originalMac">The original mac.</param>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        internal static bool ValidateMessageAuthenticationCodeInternal(byte[] key, byte[] originalMac, Stream encryptedData, long startIndex)
        {
            byte[] calculatedMac = CalculateMessageAuthenticationCode(key, encryptedData, startIndex);
            return DataTools.CompareByteArrays(calculatedMac, originalMac);
        }
    }
}
