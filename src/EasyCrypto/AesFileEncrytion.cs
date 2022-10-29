using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyCrypto
{
    /// <summary>
    /// File encryption abstraction
    /// </summary>
    public static class AesFileEncrytion
    {
        /// <summary>
        /// Encrypts the specified file.
        /// </summary>
        /// <param name="sourceFilePath">The plain text file path.</param>
        /// <param name="destinationFilePath">The encrypted file path to write to.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if file already exists.</param>
        public static void Encrypt(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                AesEncryption.Encrypt(source, key, iv, destination, token);
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
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        /// <returns>Task to await</returns>
        public static async Task EncryptAsync(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await AesEncryption.EncryptAsync(source, key, iv, destination, token);
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
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        public static void Decrypt(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                AesEncryption.Decrypt(source, key, iv, destination, token);
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
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        /// <returns>Task to await</returns>
        public static async Task DecryptAsync(string sourceFilePath, string destinationFilePath, byte[] key, byte[] iv, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);

            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await AesEncryption.DecryptAsync(source, key, iv, destination, token);
            }
        }

        /// <summary>
        /// Encrypts the with password asynchronously.
        /// </summary>
        /// <param name="sourceFilePath">The source file path to encrypt.</param>
        /// <param name="destinationFilePath">The destination file path to write to encrypted data.</param>
        /// <param name="password">The password.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if destination file already exists.</param>
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        public static void EncryptWithPassword(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);
            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                AesEncryption.EncryptWithPassword(source, password, destination, token);
            }
        }

        /// <summary>
        /// Encrypts the with password asynchronously.
        /// </summary>
        /// <param name="sourceFilePath">The source file path to encrypt.</param>
        /// <param name="destinationFilePath">The destination file path to write to encrypted data.</param>
        /// <param name="password">The password.</param>
        /// <param name="overwriteExistingFile">if set to <c>false</c> exception will be thrown if destination file already exists.</param>
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        /// <returns>Task to await</returns>
        public static async Task EncryptWithPasswordAsync(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);
            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await AesEncryption.EncryptWithPasswordAsync(source, password, destination, token);
            }
        }

        /// <summary>
        /// Decrypts the with password.
        /// </summary>
        /// <param name="sourceFilePath">The source file path to decrypt.</param>
        /// <param name="destinationFilePath">The destination file path to write to decrypted data.</param>
        /// <param name="password">The password.</param>
        /// <param name="overwriteExistingFile">if set to <c>true</c> [overwrite existing file].</param>
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        public static void DecryptWithPassword(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);
            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                AesEncryption.DecryptWithPassword(source, password, destination, token);
            }
        }

        /// <summary>
        /// Decrypts the with password asynchronously.
        /// </summary>
        /// <param name="sourceFilePath">The source file path to decrypt.</param>
        /// <param name="destinationFilePath">The destination file path to write to decrypted data.</param>
        /// <param name="password">The password.</param>
        /// <param name="overwriteExistingFile">if set to <c>true</c> [overwrite existing file].</param>
        /// <param name="token">Optional token for progress report and cancellation of the operation.</param>
        /// <returns>Task to await</returns>
        public static async Task DecryptWithPasswordAsync(string sourceFilePath, string destinationFilePath, string password, bool overwriteExistingFile, ReportAndCancellationToken token = null)
        {
            ValidateFileDestionation(destinationFilePath, overwriteExistingFile);
            using (Stream source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream destination = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await AesEncryption.DecryptWithPasswordAsync(source, password, destination, token);
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
