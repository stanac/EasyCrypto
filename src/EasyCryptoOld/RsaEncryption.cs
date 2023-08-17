using System;
using System.Security.Cryptography;
using System.Text;
using EasyCrypto.Internal;

namespace EasyCrypto
{
    /// <summary>
    /// Class for generating key pairs and encrypting and decrypting
    /// </summary>
    public static class RsaEncryption
    {
        /// <summary>
        /// Generates RSA key pair with 2048 bits
        /// </summary>
        /// <returns>Generated key pairs</returns>
        public static RsaKeyPair GenerateKeyPairs() => GenerateKeyPairs(RsaKeySizes.Rsa2048);

        /// <summary>
        /// Generates RSA key pair of desired size (note that keys larger than 4096 bits will have high impact on performance)
        /// </summary>
        /// <param name="keySize">Size of the key</param>
        /// <returns>Generated key pair</returns>
        public static RsaKeyPair GenerateKeyPairs(RsaKeySizes keySize)
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = keySize.GetIntegerKeySize();

                return new RsaKeyPair(rsa.ExportParameters(true));
            }
        }

        /// <summary>
        /// Encrypts array of bytes
        /// Size of data cannot be longer than the key used
        /// </summary>
        /// <param name="data">Data to encrypt, cannot be larger than the key used</param>
        /// <param name="key">Key to use for encryption</param>
        /// <returns>Encrypted data as byte array</returns>
        public static byte[] Encrypt(byte[] data, RsaPublicKey key)
        {
            using (var rsa = RSA.Create())
            {
                var parms = key.GetParameters();
                rsa.KeySize = parms.Modulus.Length * 8;
                rsa.ImportParameters(parms);

                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }

        /// <summary>
        /// Encrypts string
        /// Size of data cannot be longer than the key used
        /// </summary>
        /// <param name="data">Data to encrypt, cannot be larger than the key used</param>
        /// <param name="key">Key to use for encryption</param>
        /// <returns>Encrypted data as string</returns>
        public static string Encrypt(string data, RsaPublicKey key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            byte[] encrypted = Encrypt(bytes, key);

            return Convert.ToBase64String(encrypted).BeautifyBase64();
        }

        /// <summary>
        /// Decrypts array of bytes (if public key is used for encryption than private key needs to be used for decryption and vice versa)
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <param name="key">Key to use for decryption</param>
        /// <returns>Decrypted data</returns>
        public static byte[] Decrypt(byte[] data, RsaPrivateKey key)
        {
            using (var rsa = RSA.Create())
            {
                var parms = key.GetParameters();
                rsa.KeySize = parms.Modulus.Length * 8;
                rsa.ImportParameters(parms);

                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }

        /// <summary>
        /// Decrypts string (if public key is used for encryption than private key needs to be used for decryption and vice versa)
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        /// <param name="key">Key to use for decryption</param>
        /// <returns>Decrypted data</returns>
        public static string Decrypt(string data, RsaPrivateKey key)
        {
            byte[] bytes = Convert.FromBase64String(data.UglifyBase64());
            byte[] decrypted = Decrypt(bytes, key);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
