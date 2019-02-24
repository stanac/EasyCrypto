using System;
using System.Security.Cryptography;

namespace EasyCrypto
{
    public class RsaPrivateKey
    {
        public RsaPrivateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value isn't set.", nameof(key));
            }

            Key = key;
        }

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

        public int KeySize => GetParameters().Modulus.Length * 8;

        public string Key { get; }

        public RSAParameters GetParameters() => RsaKeyEncoder.Decode(Key);
    }
}
