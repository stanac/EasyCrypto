using System;
using System.IO;

namespace EasyCrypto
{
    internal class CryptoContainer
    {
        /*
         *      Format:
         *          04 bytes     - MagicNumber
         *          02 bytes     - DataVersionNumber
         *          02 bytes     - MinCompatibleDataVersionNumber
         *          16 bytes     - IV
         *          32 bytes     - Salt
         *          19 bytes     - Key check value
         *          48 bytes     - MAC
         *          ----- end of header ----- (sum: 123)
         *          xx bytes     - data
         *          
         */

        public CryptoContainer() { }

        private CryptoContainer(CryptoRequest request)
        {
            if (request.EmbedSalt)
            {
                Flags = CryptoContainerFlags.HasSalt;
                Salt = request.Salt;
            }
            else
            {
                Salt = new byte[32];
            }
            IV = request.IV;
            OutData = request.OutData;
        }

        public static CryptoContainer CreateForEncryption(CryptoRequest request)
            => new CryptoContainer(request);

        public const int MagicNumber = 212574318;
        public const short DataVersionNumber = 1;
        public const short MinCompatibleDataVersionNumber = 1;

        public CryptoContainerFlags Flags { get; set; }
        public byte[] Salt { get; private set; }
        public byte[] IV { get; private set; }
        public byte[] KeyCheckValue { get; private set; }
        public byte[] MessageAuthenticationCode { get; private set; }
        public Stream OutData { get; set; }

        public void WriteEmptyHeaderData()
        {
            byte[] data = new byte[123]; // 123 is sum of header items
            OutData.Write(data, 0, data.Length);
        }

        public void WriteChecksAndEmbededData()
        {
            OutData.Write(BitConverter.GetBytes(MagicNumber), 0, 4);
            OutData.Write(BitConverter.GetBytes(DataVersionNumber), 0, 2);
            OutData.Write(BitConverter.GetBytes(MinCompatibleDataVersionNumber), 0, 2);
            OutData.Write(IV, 0, 16);
            
        }

        // private bool EmbedSalt => Flags.HasFlag(CryptoContainerFlags.HasSalt);
    }
}
