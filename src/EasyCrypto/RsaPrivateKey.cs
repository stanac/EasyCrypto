using System;
using System.Security.Cryptography;

namespace EasyCrypto
{
    /// <summary>
    /// RSA private key
    /// </summary>
    public class RsaPrivateKey
    {
        /// <summary>
        /// Constructor accepting string key
        /// </summary>
        /// <param name="key">Key value</param>
        public RsaPrivateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value isn't set.", nameof(key));
            }

            Key = key;
        }

        /// <summary>
        /// Constructor accepting <see cref="RSAParameters"/>
        /// </summary>
        /// <param name="parameters"></param>
        public RsaPrivateKey(RSAParameters parameters)
        {
            if (parameters.D == null
                || parameters.DP == null
                || parameters.DQ == null
                || parameters.InverseQ == null
                || parameters.P == null
                || parameters.Q == null
                )
            {
                throw new ArgumentException("It looks like provided parameter is not private key", nameof(parameters));
            }

            Key = RsaKeyEncoder.Encode(parameters);
        }

        /// <summary>
        /// Key size in bytes
        /// </summary>
        public int KeySize => GetParameters().Modulus.Length * 8;

        /// <summary>
        /// Key in form of string
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets key in form of <see cref="RSAParameters"/>
        /// </summary>
        /// <returns>RSAParameters</returns>
        public RSAParameters GetParameters() => RsaKeyEncoder.Decode(Key);
    }
}
