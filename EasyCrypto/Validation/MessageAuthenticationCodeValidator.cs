using System.IO;
using System.Security.Cryptography;

namespace EasyCrypto.Validation
{
    public static class MessageAuthenticationCodeValidator
    {
        public static byte[] CalculateMessageAuthenticationCode(byte[] key, Stream encryptedData)
        {
            HMACSHA384 hmac = new HMACSHA384(key);
            long originalPosition = encryptedData.Position;
            encryptedData.Position = 0;
            byte[] mac = hmac.ComputeHash(encryptedData);
            encryptedData.Position = originalPosition;
            return mac;
        }

        public static void ValidateMessageAuthenticationCode(byte[] key, byte[] originalMac, Stream encryptedData)
        {
            byte[] calculatedMac = CalculateMessageAuthenticationCode(key, encryptedData);
            if (!DataTools.CompareByteArrays(calculatedMac, originalMac))
            {
                throw new Exceptions.DataIntegrityValidationException("Validation of Message Authentication Code (MAC) has failed. " +
                    "Most likely reason for this exception is that encrypted data was modifiled.");
            }
        }
    }
}
