using System.IO;
using EasyCrypto.Internal;

namespace EasyCrypto.Validation
{
    /// <summary>
    /// Validates for KCV
    /// </summary>
    public static class KeyCheckValueValidator
    {
        /// <summary>
        /// The _KCV data
        /// </summary>
        private static readonly byte[] _kcvData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// The _KCV length
        /// </summary>
        private const int _kcvLength = 3;

        /// <summary>
        /// Generates the key check value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Byte array, the KCV</returns>
        public static byte[] GenerateKeyCheckValue(byte[] key)
            => GenerateKeyCheckValue(key, CryptoRandom.Default.NextBytes(16));

        /// <summary>
        /// Generates the key check value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        private static byte[] GenerateKeyCheckValue(byte[] key, byte[] iv)
        {
            byte[] result;
            using (Stream inStream = new MemoryStream())
            using (Stream outStream = new MemoryStream())
            {
                inStream.Write(_kcvData, 0, _kcvData.Length);
                inStream.Position = 0;
                AesEncryption.Encrypt(new CryptoRequest
                {
                    SkipValidations = true,
                    InData = inStream,
                    OutData = outStream,
                    Key = key,
                    IV = iv
                });

                byte[] encryptedData = outStream.ToBytes();
                result = encryptedData.SkipTake(0, _kcvLength);
            }
            return InternalDataTools.JoinByteArrays(result, iv);
        }

        /// <summary>
        /// Validates the key check value. Throws <see cref="Exceptions.KeyCheckValueValidationException"/> if validation fails.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="originalKCV">The original KCV.</param>
        /// <exception cref="EasyCrypto.Exceptions.KeyCheckValueValidationException">KCV validation is unsuccessful. Most likely wrong key/password used for decryption.</exception>
        public static void ValidateKeyCheckValue(byte[] key, byte[] originalKCV)
        {
            if (!ValidateKeyCheckValueInternal(key, originalKCV))
            {
                throw new Exceptions.KeyCheckValueValidationException("KCV validation is unsuccessful. Most likely wrong key/password used for decryption.");
            }
        }

        /// <summary>
        /// For internal usage
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="originalKCV">The original KCV.</param>
        /// <returns></returns>
        internal static bool ValidateKeyCheckValueInternal(byte[] key, byte[] originalKCV)
        {
            byte[] calculatedKcv = GenerateKeyCheckValue(key, originalKCV.SkipTake(3, 16));
            return InternalDataTools.CompareByteArrays(originalKCV, calculatedKcv);
        }
    }
}
