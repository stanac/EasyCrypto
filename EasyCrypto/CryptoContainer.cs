using EasyCrypto.Exceptions;
using EasyCrypto.Validation;
using System;
using System.Diagnostics;
using System.IO;

namespace EasyCrypto
{
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
        public const short DataVersionNumber = 1;
        public const short MinCompatibleDataVersionNumber = 1;

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

        public void WriteChecksAndEmbededData()
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
            bool kcvPass = KeyCheckValueValidator.ValidateKeyCheckValueInternal(Key, KeyCheckValue);
            if (!kcvPass)
            {
                result.SetException(DataFormatValidationException.DataValidationErrors.KeyCheckValueValidationError);
                return result;
            }

            result.KeyIsValid = true;

            bool macIsValid = MessageAuthenticationCodeValidator.ValidateMessageAuthenticationCodeInternal(Key, MessageAuthenticationCode, InData, HeaderSize);
            if (!macIsValid)
            {
                result.SetException(DataFormatValidationException.DataValidationErrors.DataIntegrityValidationError);
                return result;
            }

            result.DataIntegrityIsValid = true;
            return result;
        }

        private bool EmbedSalt => Flags.HasFlag(CryptoContainerFlags.HasSalt);

        public byte[] GetIV() => GetHeaderBytes(8, 16);
        
        private byte[] GetHeaderBytes(int startIndex, int length) => _headerData.SkiptTake(startIndex, length);
        private short GetHeaderInt16(int startIndex) => BitConverter.ToInt16(_headerData, startIndex);
        private int GetHeaderInt32(int startIndex) => BitConverter.ToInt32(_headerData, startIndex);

        internal byte[] CalculateKey() => new PasswordHasher().HashPassword(_password, Salt);
    }
}
