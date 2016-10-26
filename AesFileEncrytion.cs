using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCrypto
{
    public static class AesFileEncrytion
    {
        /// <summary>
        /// Encrypts the specified file.
        /// </summary>
        /// <param name="sourceFilePath">The plain text file path.</param>
        /// <param name="destinationFilePath">The encrypted file path to write to.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if file already exists.</param>
        public static void Encrypt(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                AesEncryption.Encrypt(source, key, iv, destination);
            }
        }

        /// <summary>
        /// Encrypts the specified file asynchronously.
        /// </summary>
        /// <param name="sourceFilePath">The plain text file path.</param>
        /// <param name="destinationFilePath">The encrypted file path to write to.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if file already exists.</param>
        public static async Task EncryptAsync(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await AesEncryption.EncryptAsync(source, key, iv, destination);
            }
        }

        /// <summary>
        /// Decrypts the specified file.
        /// </summary>
        /// <param name="sourceFilePath">The encrypted file path.</param>
        /// <param name="destinationFilePath">The decrypted file path to write to.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if destination file already exists.</param>
        public static void Decrypt(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                AesEncryption.Decrypt(source, key, iv, destination);
            }
        }

        /// <summary>
        /// Decrypts the specified file asynchronously.
        /// </summary>
        /// <param name="sourceFilePath">The encrypted file path.</param>
        /// <param name="destinationFilePath">The decrypted file path to write to.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if destination file already exists.</param>
        public static async Task DecryptAsync(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await AesEncryption.DecryptAsync(source, key, iv, destination);
            }
        }

        private static void ValidateFileDestionation( string destinationFilePath, bool overwriteExistingFile)
        {
            if (!overwriteExistingFile && File.Exists(destinationFilePath))
            {
                throw new ArgumentException("Destination file already exists, set overrideExistingFile to true you want to overwrite existing file.");
            }
        }
    }
}
