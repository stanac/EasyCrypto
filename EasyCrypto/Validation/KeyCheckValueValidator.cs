using System.IO;

namespace EasyCrypto.Validation
{
    public static class KeyCheckValueValidator
    {
        private static readonly byte[] _kcvData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private const int _kcvLength = 3;

        public static byte[] GenerateKeyCheckValue(byte[] key)
            => GenerateKeyCheckValue(key, CryptoRandom.NextBytesStatic(16));

        private static byte[] GenerateKeyCheckValue(byte[] key, byte[] iv)
        {
            byte[] result = new byte[_kcvLength];
            using (Stream inStream = new MemoryStream())
            using (Stream outStream = new MemoryStream())
            {
                inStream.Write(_kcvData, 0, _kcvData.Length);
                AesEncryption.Encrypt(new CryptoRequest
                {
                    SkipValidations = true,
                    InData = inStream,
                    OutData = outStream,
                    Key = key,
                    IV = iv
                });
                outStream.Position = 0;
                outStream.Read(result, 0, result.Length);
            }
            return DataTools.JoinByteArrays(result, iv);
        }

        public static void ValidateKeyCheckValue(byte[] key, byte[] originalKCV)
        {
            byte[] calculatedKcv = GenerateKeyCheckValue(key, originalKCV.SkiptTake(3, 16));
            if (!DataTools.CompareByteArrays(originalKCV, calculatedKcv))
            {
                throw new Exceptions.KeyCheckValueValidationException("KCV validation is unsuccessful. Most likely wrong key/password used for decryption.");
            }
        }
    }
}
