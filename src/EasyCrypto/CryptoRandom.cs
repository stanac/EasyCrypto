using System;
using System.Security.Cryptography;

namespace EasyCrypto
{
    /// <summary>
    /// Cryptographic level RNG using <see cref="RandomNumberGenerator"/>.
    /// </summary>
    public class CryptoRandom : IDisposable
    {
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly bool _useBuffer;
        private readonly SlidingBuffer _buffer;

        /// <summary>
        /// Default instance
        /// </summary>
        public static CryptoRandom Default { get; } = new CryptoRandom(true);

        /// <summary>
        /// Default constructor
        /// </summary>
        public CryptoRandom()
            : this (false)
        {

        }

        /// <summary>
        /// Constructor with support for buffering
        /// </summary>
        /// <param name="useBuffer">If true less calls to underlying RNG will be placed,
        /// should improved performance in case when single instance of <see cref="CryptoRandom"/>
        /// is used multiple times, default value is false</param>
        public CryptoRandom(bool useBuffer)
        {
            if (useBuffer)
            {
                _useBuffer = true;
                _buffer = new SlidingBuffer(_rng.GetBytes);
            }
        }

        /// <summary>
        /// Returns new random bytes.
        /// </summary>
        /// <param name="length">The number for bytes to return.</param>
        /// <returns>Byte array</returns>
        [Obsolete(ObsoleteMessage.Message)]
        public static byte[] NextBytesStatic(uint length)
        {
            using (var cr = new CryptoRandom())
            {
                return cr.NextBytes(length);
            }
        }

        /// <summary>
        /// Returns byte array
        /// </summary>
        /// <param name="length">Length of array to return</param>
        /// <returns>Byte array filled with random bytes</returns>
        public byte[] NextBytes(uint length)
        {
            var array = new byte[length];
            GetRngBytes(array);
            return array;
        }

        /// <summary>
        /// Generates random int &gt;= 0
        /// </summary>
        /// <returns>Random integer</returns>
        [Obsolete(ObsoleteMessage.Message)]
        public static int NextIntStatic() => NextIntStatic(0, int.MaxValue);

        /// <summary>
        /// Generates random int &gt;= 0 and &lt; maxExclusive
        /// </summary>
        /// <param name="maxExclusive">Maximum exclusive value to return</param>
        /// <returns>Random integer</returns>
        [Obsolete(ObsoleteMessage.Message)]
        public static int NextIntStatic(int maxExclusive) => NextIntStatic(0, maxExclusive);

        /// <summary>
        /// Generates random int &gt;= minInclusive and &lt; maxExclusive
        /// </summary>
        /// <param name="minInclusive">Minimum inclusive value to return</param>
        /// <param name="maxExclusive">Maximum exclusive value to return</param>
        /// <returns>Random integer</returns>
        [Obsolete(ObsoleteMessage.Message)]
        public static int NextIntStatic(int minInclusive, int maxExclusive)
        {
            using (var cr = new CryptoRandom())
            {
                return cr.NextInt(minInclusive, maxExclusive);
            }
        }

        /// <summary>
        /// Generates random int &gt;= 0
        /// </summary>
        /// <returns>Random integer</returns>
        public int NextInt() => NextInt(0, int.MaxValue);

        /// <summary>
        /// Generates random int &gt;= 0 and &lt; maxExclusive
        /// </summary>
        /// <param name="maxExclusive">Maximum exclusive value to return</param>
        /// <returns>Random integer</returns>
        public int NextInt(int maxExclusive) => NextInt(0, maxExclusive);

        /// <summary>
        /// Generates random int &gt;= minInclusive and &lt; maxExclusive
        /// </summary>
        /// <param name="minInclusive">Minimum inclusive value to return</param>
        /// <param name="maxExclusive">Maximum exclusive value to return</param>
        /// <returns>Random integer</returns>
        public int NextInt(int minInclusive, int maxExclusive)
        {
            var array = new int[1];
            FillIntArrayWithRandomValues(array, minInclusive, maxExclusive);
            return array[0];
        }

        /// <summary>
        /// Generates random double between 0.0 and 1.0
        /// </summary>
        /// <returns>Random double</returns>
        public double NextDouble() => (double)NextInt() / int.MaxValue;

        /// <summary>
        /// Return random double between 0 and 1
        /// </summary>
        /// <returns>Double between 0 and 1</returns>
        [Obsolete(ObsoleteMessage.Message)]
        public static double NextDoubleStatic()
        {
            using (var cr = new CryptoRandom())
            {
                return cr.NextDouble();
            }
        }

        /// <summary>
        /// Fills array of integers with random values
        /// </summary>
        /// <param name="arrayToFill">Array to fill with random integers</param>
        /// <param name="minInclusive">Minimum inclusive value to return</param>
        /// <param name="maxExclusive">Maximum exclusive value to return</param>
        public void FillIntArrayWithRandomValues(int[] arrayToFill, int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive) throw new ArgumentException($"{nameof(minInclusive)} must be less than {nameof(maxExclusive)}.");

            byte[] randomBytes = new byte[arrayToFill.Length * sizeof(int)];
            GetRngBytes(randomBytes);
            for (int i = 0; i < arrayToFill.Length; i++)
            {
                int temp = BitConverter.ToInt32(randomBytes, i * sizeof(int));
                unchecked
                {
                    temp = (int)(((uint)temp) / 2);
                }
                int range = maxExclusive - minInclusive;
                temp = (temp % range) + minInclusive;
                arrayToFill[i] = temp;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _rng.Dispose();
        }

        private void GetRngBytes(byte[] arrayToFill)
        {
            if (_useBuffer)
            {
                _buffer.GetData(arrayToFill);
            }
            else
            {
                _rng.GetBytes(arrayToFill);
            }
        }
    }
}
