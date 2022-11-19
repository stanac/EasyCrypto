using System;

namespace EasyCrypto
{

    internal class PasswordHashContainer
    {
        private const int ContainerVersion = 4596;
        private readonly int _algorithmId;
        private readonly byte[] _algorithmData;
        private readonly byte[] _salt;
        private readonly byte[] _hash;

        public PasswordHashContainer(int algorithmId, byte[] algorithmData, byte[] salt, byte[] hash)
        {
            _algorithmId = algorithmId;
            _algorithmData = algorithmData;
            _salt = salt;
            _hash = hash;
        }

        public static PasswordHashContainer CreateForAlgorithmId2(int iterations, byte[] salt, byte[] hash)
        {
            return new PasswordHashContainer(PasswordHasherAndValidator.AlgorithmId, BitConverter.GetBytes(iterations), salt, hash);
        }

        public int GetIterations()
        {
            return BitConverter.ToInt32(_algorithmData, 0);
        }
        
        /*
            Binary format:

            Container version:      4 bytes (also a magic number)
            Algorithm id:           4 bytes
            Salt length:            4 bytes
            Algorithm data length:  4 bytes
            Salt:                   x bytes
            Hash:                   x bytes
         */

        public byte[] ToBytes()
        {
            const int staticDataLength = 4 + 4 + 4 + 4;

            byte[] result = new byte[staticDataLength + _salt.Length + _hash.Length];

            byte[] version = BitConverter.GetBytes(ContainerVersion);
            byte[] algoId = BitConverter.GetBytes(_algorithmId);
            byte[] saltLength = BitConverter.GetBytes(_salt.Length);
            byte[] algoDataLength = BitConverter.GetBytes(_algorithmData.Length);

            version.CopyTo(result, 0);
            algoId.CopyTo(result, 4);
            saltLength.CopyTo(result, 8);
            algoDataLength.CopyTo(result, 12);
            _salt.CopyTo(result, 16);

            int hashPosition = staticDataLength + _salt.Length;
            _hash.CopyTo(result, hashPosition);

            return result;
        }

        public static PasswordHashContainer Parse(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.Length < 17)
            {
                throw new ArgumentException("Data length too low", nameof(data));
            }

            int containerVersion = BitConverter.ToInt32(data, 0);
            EnsureContainerVersionIsSupported(containerVersion);

            int algoId = BitConverter.ToInt32(data, 4);
            EnsureAlgorithmIdIsSupported(4);

            int saltLength = 

        }

        private static void EnsureContainerVersionIsSupported(int containerVersion)
        {
            if (containerVersion != ContainerVersion)
            {
                throw new InvalidOperationException("Container version mismatch");
            }
        }

        private static void EnsureAlgorithmIdIsSupported(int algoId)
        {
            if (algoId != PasswordHasherAndValidator.AlgorithmId)
            {
                throw new InvalidOperationException($"Unsupported algorithm version `{algoId}`");
            }
        }

        private static void EnsureSaltLengthValid(int saltLength)
        {
            if (saltLength < 1)
            {
                throw new InvalidOperationException("Invalid salt length");
            }
        }
    }
}
