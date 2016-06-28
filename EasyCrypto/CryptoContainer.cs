using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCrypto
{
    internal class CryptoContainer
    {
        /*
         *      Format:
         *          04 bytes     - MagicNumber
         *          04 bytes     - DataVersionNumber
         *          04 bytes     - MinCompatibleDataVersionNumber
         *          03 bytes     - Key check value
         *          16 bytes     - 
         *          
         */
        public const int MagicNumber = 212574318;
        public const int DataVersionNumber = 1;
        public const int MinCompatibleDataVersionNumber = 1;

        public CryptoContainerFlags Flags { get; set; }
        public byte[] Salt { get; set; }
        public byte[] IV { get; set; }
        public byte[] KeyCheckValue { get; set; }
        public byte[] MessageAuthenticationCode { get; set; }
        public Stream EncryptedData { get; set; }
    }
}
