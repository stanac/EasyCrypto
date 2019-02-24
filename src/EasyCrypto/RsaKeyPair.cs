using System.Security.Cryptography;

namespace EasyCrypto
{
    /// <summary>
    /// RSA key pairs
    /// </summary>
    public class RsaKeyPair
    {
        /// <summary>
        /// Constructor accepts private key
        /// </summary>
        /// <param name="privateKey">Private key</param>
        public RsaKeyPair(RsaPrivateKey privateKey)
        {
            PrivateKey = privateKey ?? throw new System.ArgumentNullException(nameof(privateKey));
            PublicKey = new RsaPublicKey(privateKey);
        }

        /// <summary>
        /// Constructor, accepts RSAParameters
        /// </summary>
        /// <param name="rsaParams">RSA parameters</param>
        public RsaKeyPair(RSAParameters rsaParams)
        {
            PublicKey = new RsaPublicKey(rsaParams);
            PrivateKey = new RsaPrivateKey(rsaParams);
        }

        /// <summary>
        /// Public key
        /// </summary>
        public RsaPublicKey PublicKey { get; }

        /// <summary>
        /// Private key
        /// </summary>
        public RsaPrivateKey PrivateKey { get; }
    }
}
