using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasyCrypto
{
    public class PasswordHasher
    {
        public uint HashLengthInBytes { get; private set; }
        public uint SaltLengthInBytes { get; private set; }
        public uint HashIterations { get; private set; }

        /// <summary>
        /// Default constructor with 16 bytes of salt, 16 bytes of hash and 25000 hash iterations
        /// </summary>
        public PasswordHasher()
            : this(16)
        { }

        /// <summary>
        /// Construction that accepts size of hash and salt
        /// </summary>
        /// <param name="hashAndSaltLengthsInBytes">Length of hash and salt in bytes, 
        /// must be one of: 8, 16, 32, 64, 128 (128 might be overkill)</param>
        public PasswordHasher(uint hashAndSaltLengthsInBytes)
            : this(hashAndSaltLengthsInBytes, 25000)
        { }

        /// <summary>
        /// Construction that accepts size of hash and salt and number of itterations
        /// </summary>
        /// <param name="hashAndSaltLengthsInBytes">Length of hash and salt in bytes, 
        /// must be one of: 8, 16, 32, 64, 128 (128 might be overkill)</param>
        /// <param name="hashIterations">Number of hash itterations</param>
        public PasswordHasher(uint hashAndSaltLengthsInBytes, uint hashIterations)
        {
            if (!(new[] { 8, 16, 32, 64, 128 }.Contains((int)hashAndSaltLengthsInBytes)))
            {
                throw new ArgumentException($"{nameof(hashAndSaltLengthsInBytes)} must be 8, 16, 32 or 64");
            }

            HashLengthInBytes = hashAndSaltLengthsInBytes;
            SaltLengthInBytes = hashAndSaltLengthsInBytes;
            HashIterations = hashIterations;
        }

        #region Hashing password

        /// <summary>
        /// Hashes password
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="salt">Salt to use for hashing</param>
        /// <returns>Byte[], hashed password</returns>
        public byte[] HashPassword(string password, byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("password cannot be null, empty or white space");
            }
            if (salt == null || salt.Length != (int)SaltLengthInBytes)
            {
                throw new ArgumentException("salt cannot be null and must be in length equal to value set in constructor, default is 16 bytes");
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = (int)HashIterations;
                return pbkdf2.GetBytes((int)HashLengthInBytes);
            }
        }

        /// <summary>
        /// Hashes password and generates new salt
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="salt">Generated salt</param>
        /// <returns>Byte[], Hash of the password</returns>
        public byte[] HashPasswordAndGenerateSalt(string password, out byte[] salt)
        {
            salt = GenerateRandomSalt();
            return HashPassword(password, salt);
        }

        /// <summary>
        /// Hashes password and adds salt to output, can be later validated by <see cref="ValidatePasswordWithEmbededSalt(string, byte[])"/>
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <returns>Byte[], hashed password with embeded salt</returns>
        public byte[] HashPasswordAndGenerateEmbededSalt(string password)
        {
            byte[] salt;
            byte[] hash = HashPasswordAndGenerateSalt(password, out salt);

            byte[] retValue = new byte[2 * sizeof(int) + salt.Length + hash.Length];
            BitConverter.GetBytes(salt.Length).CopyTo(retValue, 0);
            BitConverter.GetBytes(hash.Length).CopyTo(retValue, sizeof(int));
            salt.CopyTo(retValue, 2 * sizeof(int));
            hash.CopyTo(retValue, 2 * sizeof(int) + salt.Length);

            return retValue;
        }

        /// <summary>
        /// Same as <see cref="HashPasswordAndGenerateEmbededSalt(string)"/> but returns base64 string
        /// Can be validated by <see cref="ValidatePasswordWithEmbededSaltAsString(string, string)"/>
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <returns>String, hashed password with embeded salt</returns>
        public string HashPasswordAndGenerateEmbededSaltAsString(string password)
            => Convert.ToBase64String(HashPasswordAndGenerateEmbededSalt(password));

        #endregion Hashing password

        #region Validating password

        /// <summary>
        /// Validates password against provided hash
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <param name="hash">Hash of check the password against</param>
        /// <param name="salt">Salt used for original hashing</param>
        /// <returns>Bool, true if password is valid</returns>
        public bool ValidatePassword(string password, byte[] hash, byte[] salt)
        {
            byte[] newHash = HashPassword(password, salt);
            return DataTools.CompareByteArrays(hash, newHash);
        }

        /// <summary>
        /// Validates password against provided salt with embeded hash
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <param name="hashAndEmbededSalt">Hash with embeded salt</param>
        /// <returns>Bool, true if password is valid</returns>
        public bool ValidatePasswordWithEmbededSalt(string password, byte[] hashAndEmbededSalt)
        {
            int saltLength = BitConverter.ToInt32(hashAndEmbededSalt, 0);
            int hashLength = BitConverter.ToInt32(hashAndEmbededSalt, sizeof(int));

            Debug.Assert(hashAndEmbededSalt.Length == saltLength + hashLength + 2 * sizeof(int), "hashAndEmbededSalt is not of valid size");

            byte[] salt = hashAndEmbededSalt.Skip(2 * sizeof(int)).Take(saltLength).ToArray();
            byte[] hash = hashAndEmbededSalt.Skip(2 * sizeof(int) + saltLength).Take(hashLength).ToArray();

            return ValidatePassword(password, hash, salt);
        }

        /// <summary>
        /// Same as <see cref="ValidatePasswordWithEmbededSalt(string, byte[])"/> but accepts string for hashWithEmbededPassword
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <param name="hashAndEmbededSalt">Hash with embeded salt</param>
        /// <returns>Bool, true if password is valid</returns>
        public bool ValidatePasswordWithEmbededSaltAsString(string password, string hashAndEmbededSalt)
            => ValidatePasswordWithEmbededSalt(password, Convert.FromBase64String(hashAndEmbededSalt));

        #endregion Validating password

        public byte[] GenerateRandomSalt() => CryptoRandom.NextBytesStatic(SaltLengthInBytes);

    }
}
