using System;
using System.Security.Cryptography;

namespace EasyCrypto
{
    /// <summary>
    /// RSA public key
    /// </summary>
    public class RsaPublicKey
    {
        /// <summary>
        /// Constructor, accepts encoded key, only stores public part of the key
        /// </summary>
        /// <param name="key">Encoded key</param>
        public RsaPublicKey(string key) : this(RsaKeyEncoder.Decode(key))
        {

        }

        /// <summary>
        /// Constructor accepts private key, only stores public part of the key
        /// </summary>
        /// <param name="key">Private key</param>
        public RsaPublicKey(RsaPrivateKey key) : this(key?.Key)
        {

        }

        /// <summary>
        /// Constructor accepts parameters, only stores public part of the key
        /// </summary>
        /// <param name="parameters">RSA parameters</param>
        public RsaPublicKey(RSAParameters parameters)
        {
            if (parameters.Modulus == null
                || parameters.Exponent == null
                )
            {
                throw new ArgumentException("It looks like provided parameters are not set.", nameof(parameters));
            }

            parameters.D = null;
            parameters.DP = null;
            parameters.DQ = null;
            parameters.InverseQ = null;
            parameters.P = null;
            parameters.Q = null;

            Key = RsaKeyEncoder.Encode(parameters);
        }

        public int KeySize => GetParameters().Modulus.Length * 8;

        public string Key { get; }

        public RSAParameters GetParameters() => RsaKeyEncoder.Decode(Key);
    }
}
