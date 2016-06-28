using System.IO;
using System.Security.Cryptography;

namespace EasyCrypto.Validation
{
    public static class MessageAuthenticationCodeValidator
    {
        public static byte[] CalculateMessageAuthenticationCode(byte[] key, Stream encryptedData, long startIndex = 0)
        {
            HMACSHA384 hmac = new HMACSHA384(key);
            long originalPosition = encryptedData.Position;
            encryptedData.Position = startIndex;
            byte[] mac = hmac.ComputeHash(encryptedData);
            encryptedData.Position = originalPosition;
            return mac;
        }

        public static void ValidateMessageAuthenticationCode(byte[] key, byte[] originalMac, Stream encryptedData, long startIndex = 0)
        {
            if (!ValidateMessageAuthenticationCodeInternal(key, originalMac, encryptedData, startIndex))
            {
                throw new Exceptions.DataIntegrityValidationException("Validation of Message Authentication Code (MAC) has failed. " +
                    "Most likely reason for this exception is that encrypted data was modifiled.");
            }
        }

        internal static bool ValidateMessageAuthenticationCodeInternal(byte[] key, byte[] originalMac, Stream encryptedData, long startIndex)
        {
            byte[] calculatedMac = CalculateMessageAuthenticationCode(key, encryptedData, startIndex);
            return DataTools.CompareByteArrays(calculatedMac, originalMac);
        }
    }
}
