using System;
using System.Collections.Generic;
using System.IO;

namespace EasyCrypto
{
    internal class CryptoRequest
    {
        public Stream InData { get; set; }
        public Stream OutData { get; set; }
        public bool EmbedIV { get; set; }
        public bool EmbedSalt { get; set; }
        public byte[] Salt { get; set; }
        public byte[] IV { get; set; }
        public bool SkipValidations { get; set; }
        public byte[] Key { get; set; }
        public string Password { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; }
        public ReportAndCancellationToken Token { get; set; }

        public CryptoContainer ValidateEncryption()
        {
            if (Key.Length != 32) throw new ArgumentException("Key must be 32 bytes long.");
            if (IV == null || IV.Length != 16) throw new ArgumentException("IV must be 16 bytes in length");

            CryptoContainer container = null;
            if (!SkipValidations)
            {
                container = CryptoContainer.CreateForEncryption(this);
                container.WriteEmptyHeaderData();
            }

            return container;
        }

        public CryptoContainer ValidateDecrypt(CryptoRequest request)
        {
            CryptoContainer container = null;
            if (!request.SkipValidations)
            {
                container = CryptoContainer.CreateForDecryption(request);
                var validationResult = container.ReadAndValidateDataForDecryption();
                if (!validationResult.IsValid)
                {
                    throw validationResult.ExceptionToThrow ?? new Exception("Unknown error");
                }
                request.IV = container.GetIV();
                if (request.Password != null)
                {
                    request.Key = container.CalculateKey();
                }
            }

            if (request.Key == null || request.Key.Length != 32) throw new ArgumentException("Key must be 32 bytes long.");
            if (request.IV == null || request.IV.Length != 16) throw new ArgumentException($"IV must be 16 bytes in length");

            return container;
        }
    }
}
