using System.IO;

namespace EasyCrypto.Validation
{
    public static class KeyCheckValueValidator
    {
        private static readonly byte[] _kcvData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private const int _kcvLength = 3;

        public static byte[] GenerateKeyCheckValue(byte[] key)
        {
            byte[] result = new byte[_kcvLength];
            byte[] iv = CryptoRandom.NextBytesStatic(16);
            using (Stream inStream = new MemoryStream())
            using (Stream outStream = new MemoryStream())
            {
                inStream.Write(_kcvData, 0, _kcvData.Length);
                AesEncryption.Encrypt(true, inStream, key, iv, outStream);
                outStream.Position = 0;
                outStream.Read(result, 0, result.Length);
            }
            return result;
        }

        public static void ValidateKeyCheckValue(byte[] key, byte[] originalKCV)
        {
            byte[] calculatedKcv = GenerateKeyCheckValue(key);
            if (!PasswordHasher.CompareByteArrays(originalKCV, calculatedKcv))
            {
                throw new Exceptions.KeyCheckValueValidationException("KCV validation is unsuccessful. Most likely wrong key/password used for decryption.");
            }
        }
    }
}
