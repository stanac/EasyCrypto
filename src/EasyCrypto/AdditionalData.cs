using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyCrypto
{
    /// <summary>
    /// Additional data to add to the encrypted, this data is not protected (but not saved in plain text either)
    /// For example can be used for password hint, date of encryption etc...
    /// </summary>
    internal class AdditionalData
    {
        private static readonly byte[] AdditionalDataKey = { 29, 173, 113, 233, 72, 224, 33, 3, 159, 29, 79, 5, 174, 168, 182, 192, 18, 204, 29, 222, 103, 183, 101, 113, 185, 220, 180, 47, 94, 75, 17, 250 };
        private static readonly byte[] AdditionalDataIv = { 45, 134, 211, 82, 19, 64, 57, 6, 239, 93, 200, 99, 183, 53, 148, 189 };
        private static readonly byte[] EmptyBytes = new byte[0];

        public AdditionalData() { }
        public AdditionalData(Dictionary<string, string> data)
        {
            Data = data ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Dictionary that will contain additional data
        /// </summary>
        public Dictionary<string, string> Data { get; private set; } = new Dictionary<string, string>();

        internal byte[] GetBytes()
        {
            byte[] data = Serialize(Data);
            byte[] encrypted = EncryptAdditionalData(data);
            return encrypted;
        }

        internal static AdditionalData LoadFromBytes(byte[] data)
        {
            data = DecryptAddtionalData(data);
            Dictionary<string, string> deserialized = Deserialize(data);
            return new AdditionalData(deserialized);
        }

        private static byte[] EncryptAdditionalData(byte[] data)
        {
            byte[] encrypted = AesEncryption.HandleByteToStream(data, (inStream, outStream) =>
            {
                AesEncryption.Encrypt(new CryptoRequest
                {
                    InData = inStream,
                    OutData = outStream,
                    Key = AdditionalDataKey,
                    IV = AdditionalDataIv,
                    SkipValidations = true,
                    EmbedIV = false,
                    EmbedSalt = false
                });
            });
            return encrypted;
        }

        private static byte[] DecryptAddtionalData(byte[] data)
        {
            byte[] decrypted = AesEncryption.HandleByteToStream(data, (inStream, outStream) =>
            {
                AesEncryption.Decrypt(new CryptoRequest
                {
                    InData = inStream,
                    OutData = outStream,
                    Key = AdditionalDataKey,
                    IV = AdditionalDataIv,
                    SkipValidations = true,
                    EmbedIV = false,
                    EmbedSalt = false
                });
            });
            return decrypted;
        }

        private static byte[] Serialize(Dictionary<string, string> data)
        {
            if (data == null || data.Count == 0)
            {
                return EmptyBytes;
            }
            var filtered = data.Where(x => !string.IsNullOrEmpty(x.Key) && !string.IsNullOrEmpty(x.Value));
            if (filtered == null)
            {
                return EmptyBytes;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in filtered)
            {
                sb
                    .Append(StringToBase64(item.Key))
                    .Append(":")
                    .Append(StringToBase64(item.Value))
                    .Append(";");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static Dictionary<string, string> Deserialize(byte[] data)
        {
            Dictionary<string, string> retData = new Dictionary<string, string>();
            if (data == null || data.Length == 0)
            {
                return retData;
            }
            string dataString = Encoding.UTF8.GetString(data);
            string[] parts = dataString.Split(";".ToCharArray());
            foreach (var part in parts.Where(x => x.Length > 0))
            {
                string[] keyValue = part.Split(":".ToCharArray());
                string key = Base64ToString(keyValue[0]);
                string value = Base64ToString(keyValue[1]);
                retData[key] = value;
            }
            return retData;
        }

        private static string StringToBase64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        private static string Base64ToString(string base64) => Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }
}
