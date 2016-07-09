using EasyCrypto.Exceptions;
using EasyCrypto.Validation;
using System;
using System.Diagnostics;
using System.IO;

namespace EasyCrypto
{
    /// <summary>
    /// Used internally to check for format and metadata (header data)
    /// </summary>
    internal class CryptoContainer
    {
        /*
         *      Format:
         *          04 bytes    00  - MagicNumber
         *          02 bytes    04  - DataVersionNumber
         *          02 bytes    06  - MinCompatibleDataVersionNumber
         *          16 bytes    08  - IV
         *          32 bytes    24  - Salt
         *          19 bytes    56  - Key check value
         *          48 bytes    75  - MAC
         *          04 bytes   123  - Additional header data length
         *          xx bytes   127  - Additional data
         *          ----- end of header ----- (sum: 127)
         *          xx bytes     - additional header data (0 for version 1)
         *          xx bytes     - data
         *          
         */

        private readonly string _password;

        private byte[] _headerData;
        private const int HeaderSize = 127;

        public CryptoContainer() { }

        private CryptoContainer(CryptoRequest request, bool createForEncryption)
        {
            if (request.EmbedSalt)
            {
                Flags = CryptoContainerFlags.HasSalt;
                Salt = request.Salt;
            }
            else
            {
                Salt = new byte[32];
            }
            if (!createForEncryption)
            {
                Salt = request.Salt;
            }
            IV = request.IV;
            if (createForEncryption)
            {
                OutData = request.OutData;
            }
            else
            {
                InData = request.InData;
                long currentPosition = InData.Position;
                InData.Position = 0;
                _headerData = new byte[HeaderSize];
                InData.Read(_headerData, 0, HeaderSize);
                InData.Position = 0;
            }
            Key = request.Key;
            _password = request.Password;
        }
        
        public static CryptoContainer CreateForEncryption(CryptoRequest request)
            => new CryptoContainer(request, true);

        public static CryptoContainer CreateForDecryption(CryptoRequest request)
            => new CryptoContainer(request, false);

        public const int MagicNumber = 212574318;
        public const short DataVersionNumber = 3;               // number was incremented, there was a bug
        public const short MinCompatibleDataVersionNumber = 3;  // v2: KCV was calculating wrong in V1 it was always 000
                                                                // v3: switched from zero padding for unvalidated data to PKCS7

        public CryptoContainerFlags Flags { get; private set; }
        public byte[] Salt { get; private set; }
        public byte[] IV { get; private set; }
        public byte[] KeyCheckValue { get; private set; }
        public byte[] MessageAuthenticationCode { get; private set; }
        public byte[] Key { get; private set; }
        public Stream OutData { get; private set; }
        public Stream InData { get; set; }

        public int HeaderTotalSize { get; private set; }
        public int ReadDataVersionNumber { get; private set; }
        public int ReadMinCompatibleDataVersionNumber { get; private set; }

        public void WriteEmptyHeaderData()
        {
            byte[] data = new byte[HeaderSize];
            OutData.Write(data, 0, data.Length);
        }

        public void WriteChecksAndEmbeddedData()
        {
            OutData.Position = 0;
            OutData.Write(BitConverter.GetBytes(MagicNumber), 0, 4);
            OutData.Write(BitConverter.GetBytes(DataVersionNumber), 0, 2);
            OutData.Write(BitConverter.GetBytes(MinCompatibleDataVersionNumber), 0, 2);
            OutData.Write(IV, 0, 16);
            OutData.Write(Salt, 0, 32);

            var kcv = KeyCheckValueValidator.GenerateKeyCheckValue(Key);
            OutData.Write(kcv, 0, 19);

            long currentPosition = OutData.Position;
            var mac = MessageAuthenticationCodeValidator.CalculateMessageAuthenticationCode(Key, OutData, HeaderSize);
            Debug.Assert(currentPosition == OutData.Position, "Stream position is changed after generating MAC");
            OutData.Write(mac, 0, 48);
            OutData.Position = 0;
        }

        public ValidationResult ReadAndValidateDataForDecryption()
            => ReadAndValidateDataForDecryption(false);

        private ValidationResult ReadAndValidateDataForDecryption(bool skipKeyCheck)
        {
            var result = new ValidationResult();

            // InData.Position = 0;
            if (InData.Length < HeaderSize)
            {
                result.SetException(DataFormatValidationException.DataValidationErrors.DataIsTooShort);
                return result;
            }

            int magic = GetHeaderInt32(0);
            short dataVersion = GetHeaderInt16(4);
            short minDataVersion = GetHeaderInt16(6);
            IV = GetIV();
            byte[] salt = GetHeaderBytes(24, 32);
            KeyCheckValue = GetHeaderBytes(56, 19);
            MessageAuthenticationCode = GetHeaderBytes(75, 48);
            int additionalDataLength = GetHeaderInt32(123); // additional data not supported in this version
            if (EmbedSalt)
            {
                Salt = salt;
                Key = new PasswordHasher().HashPassword(_password, salt);
            }
            else
            {
                salt = Salt;
            }

            if (magic != MagicNumber)
            {
                result.SetException(DataFormatValidationException.DataValidationErrors.InvalidMagicNumber);
                return result;
            }
            result.DataFormatIsValid = true;

            if (minDataVersion != MinCompatibleDataVersionNumber)
            {
                result.SetException(DataFormatValidationException.DataValidationErrors.UnsupportedDataVersion);
                return result;
            }
            result.DataFormatVersionIsValid = true;
            result.DataFormatVersionIsExact = dataVersion == DataVersionNumber;
            
            InData.Position = HeaderSize + additionalDataLength;
            HeaderTotalSize = (int)InData.Position;
            if (!skipKeyCheck)
            {
                bool kcvPass = KeyCheckValueValidator.ValidateKeyCheckValueInternal(Key, KeyCheckValue);
                if (!kcvPass)
                {
                    result.SetException(DataFormatValidationException.DataValidationErrors.KeyCheckValueValidationError);
                    return result;
                }
            }

            result.KeyIsValid = true;

            if (!skipKeyCheck)
            {
                bool macIsValid = MessageAuthenticationCodeValidator.ValidateMessageAuthenticationCodeInternal(Key, MessageAuthenticationCode, InData, HeaderSize);
                if (!macIsValid)
                {
                    result.SetException(DataFormatValidationException.DataValidationErrors.DataIntegrityValidationError);
                    return result;
                }
            }
            result.DataIntegrityIsValid = true;
            return result;
        }

        private bool EmbedSalt => Flags.HasFlag(CryptoContainerFlags.HasSalt);

        public byte[] GetIV() => GetHeaderBytes(8, 16);
        
        private byte[] GetHeaderBytes(int startIndex, int length) => _headerData.SkipTake(startIndex, length);
        private short GetHeaderInt16(int startIndex) => BitConverter.ToInt16(_headerData, startIndex);
        private int GetHeaderInt32(int startIndex) => BitConverter.ToInt32(_headerData, startIndex);

        internal byte[] CalculateKey() => new PasswordHasher().HashPassword(_password, Salt);
        
        internal static byte[] ReadAdditionalData(Stream encryptedData)
        {
            long position = encryptedData.Position;
            ValidateCryptoContainer(encryptedData);
            encryptedData.Position = 123;
            byte[] temp = new byte[4];
            encryptedData.Read(temp, 0, 4);
            int additionalDataLength = BitConverter.ToInt32(temp, 0);
            if (additionalDataLength == 0)
            {
                return new byte[0];
            }

            temp = new byte[additionalDataLength];
            encryptedData.Read(temp, 0, temp.Length);
            encryptedData.Position = position;
            return temp;
        }

        internal static void WriteAdditionalData(Stream encryptedData, byte[] dataBytes, Stream destination)
        {
            long position = encryptedData.Position;
            ValidateCryptoContainer(encryptedData);
            encryptedData.Position = 0;

            byte[] header = new byte[HeaderSize - 4];
            encryptedData.Read(header, 0, header.Length);
            destination.Write(header, 0, header.Length);
            destination.Write(BitConverter.GetBytes(dataBytes.Length), 0, 4);
            destination.Write(dataBytes, 0, dataBytes.Length);

            byte[] buffer = new byte[4 * 1024];
            int read;
            while ((read = encryptedData.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
            }
        }

        private static void ValidateCryptoContainer(Stream encryptedData)
        {
            encryptedData.Position = 0;
            var container = CreateForDecryption(new CryptoRequest
            {
                InData = encryptedData,
                Key = new byte[32],
                IV = new byte[16]
            });
            var validationResult = container.ReadAndValidateDataForDecryption(true);
            if (!validationResult.IsValid) throw validationResult.ExceptionToThrow;
        }
    }
}
