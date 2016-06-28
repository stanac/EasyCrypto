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
    }
}
