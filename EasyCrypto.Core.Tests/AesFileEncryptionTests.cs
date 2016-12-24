using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyCrypto.Tests
{
    public class AesFileEncryptionTests
    {
        [Fact]
        public async Task EncryptedFileCanBeDecrypted()
        {
            for (int i = 0; i < 5; i++)
            {
                await AssertFileEncryption(1024, true);
                await AssertFileEncryption(1024, false);
            }
        }

        //[Fact]
        //public async Task GigabyteFileCanBeEncryptedAndDecrypted()
        //{
        //    await AssertFileEncryption(1024 * 1024 * 1024, true);
        //    await AssertFileEncryption(1024 * 1024 * 1024, false);
        //}

        [Fact]
        public async Task EncryptedFileWithPasswordCanBeDecrypted()
        {
            for (int i = 0; i < 5; i++)
            {
                await AssertFileEncryptionWithPassword(1024, true);
                await AssertFileEncryptionWithPassword(1024, false);
            }
        }

        //[Fact]
        //public async Task GigabyteEncryptedFileWithPasswordCanBeDecrypted()
        //{
        //    string file = $"D:\\testtime - {Guid.NewGuid()}.txt";
        //    File.Create(file);
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    await AssertFileEncryptionWithPassword(370 * 1024 * 1024, true);
        //    sw.Stop();
        //    File.AppendAllText(file, $"elapsed {sw.Elapsed.TotalMinutes}:{sw.Elapsed.Seconds}");
        //    sw.Restart();
        //    await AssertFileEncryptionWithPassword(370 * 1024 * 1024, false);
        //    Console.WriteLine($"not async gigabyte write {sw.Elapsed}");
        //    sw.Stop();
        //}
        
        private async Task AssertFileEncryptionWithPassword(int fileLength, bool async)
        {
            string pass = PasswordGenerator.GenerateStatic();

            string plainTextFile = WriteToTempFile(fileLength);
            string encryptedFile = Path.GetRandomFileName();
            string decryptedFile = Path.GetRandomFileName();

            if (async)
            {
                await AesFileEncrytion.EncryptWithPasswordAsync(plainTextFile, encryptedFile, pass, true);
                await AesFileEncrytion.DecryptWithPasswordAsync(encryptedFile, decryptedFile, pass, true);
            }
            else
            {
                AesFileEncrytion.EncryptWithPassword(plainTextFile, encryptedFile, pass, true);
                AesFileEncrytion.DecryptWithPassword(encryptedFile, decryptedFile, pass, true);
            }

            try
            {
                AssertFileContent(plainTextFile, decryptedFile);
            }
            finally
            {
                File.Delete(plainTextFile);
                File.Delete(encryptedFile);
                File.Delete(decryptedFile);
            }
        }

        private async Task AssertFileEncryption(int fileLength, bool async)
        {
            byte[] iv = CryptoRandom.NextBytesStatic(16);
            byte[] key = CryptoRandom.NextBytesStatic(32);

            string plainTextFile = WriteToTempFile(fileLength);
            string encryptedFile = Path.GetRandomFileName();
            string decryptedFile = Path.GetRandomFileName();

            if (async)
            {
                await AesFileEncrytion.EncryptAsync(plainTextFile, encryptedFile, key, iv, true);
                await AesFileEncrytion.DecryptAsync(encryptedFile, decryptedFile, key, iv, true);
            }
            else
            {
                AesFileEncrytion.Encrypt(plainTextFile, encryptedFile, key, iv, true);
                AesFileEncrytion.Decrypt(encryptedFile, decryptedFile, key, iv, true);
            }

            try
            {
                AssertFileContent(plainTextFile, decryptedFile);
            }
            finally
            {
                File.Delete(plainTextFile);
                File.Delete(encryptedFile);
                File.Delete(decryptedFile);
            }
        }

        private string WriteToTempFile(int length)
        {
            var filePath = Path.GetTempFileName();
            byte[] buffer;
            int written = 0;
            int bufferSize = 4 * 1024;
            int toWrite;
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            using (CryptoRandom rand = new CryptoRandom())
            {
                buffer = rand.NextBytes(4 * 1024);
                while (written < length)
                {
                    toWrite = (written + bufferSize < length) ? bufferSize : length - written;
                    fileStream.Write(buffer, 0, toWrite);
                    written += toWrite;
                }
            }
            return filePath;
        }

        private bool AssertFileContent(string file1, string file2)
        {
            int bufferSize = 4 * 1024;
            byte[] buffer1 = new byte[bufferSize];
            byte[] buffer2 = new byte[bufferSize];
            
            using (Stream s1 = new FileStream(file1, FileMode.Open))
            using (Stream s2 = new FileStream(file2, FileMode.Open))
            {
                if (s1.Length != s2.Length) return false;
                int toRead = s1.Length - s1.Position > bufferSize ? bufferSize : (int)s1.Length - (int)s1.Position;

                while (s1.Position < s1.Length - 1)
                {
                    s1.Read(buffer1, 0, toRead);
                    s2.Read(buffer2, 0, toRead);

                    if (!AssertByteArrays(buffer1, buffer2, toRead))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private bool AssertByteArrays(byte[] b1, byte[] b2, int length)
        {
            if (b1.Length != b2.Length) return false;

            for (int i = 0; i < length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }

            return true;
        }
        
    }
}
