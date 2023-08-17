using System;

namespace EasyCrypto
{
    /// <summary>
    /// Thread safe regular random, inherits <see cref="Random"/>
    /// </summary>
    public class ThreadSafeRandom : Random
    {
        private readonly Random _rnd = new Random(GetSeed());
        private readonly object _sync = new object();

        /// <summary>
        /// Default shared instance of <see cref="ThreadSafeRandom"/>
        /// </summary>
        public static ThreadSafeRandom Default = new ThreadSafeRandom();

        /// <inheritdoc cref="Random"/>
        public override int Next()
        {
            lock (_sync)
            {
                return _rnd.Next();
            }
        }

        /// <inheritdoc cref="Random"/>
        public override int Next(int maxValue)
        {
            lock (_sync)
            {
                return _rnd.Next(maxValue);
            }
        }

        /// <inheritdoc cref="Random"/>
        public override int Next(int minValue, int maxValue)
        {
            lock (_sync)
            {
                return base.Next(minValue, maxValue);
            }
        }

        /// <inheritdoc cref="Random"/>
        public override void NextBytes(byte[] buffer)
        {
            lock (_sync)
            {
                base.NextBytes(buffer);
            }
        }

        /// <inheritdoc cref="Random"/>
        public override double NextDouble()
        {
            lock (_sync)
            {
                return base.NextDouble();
            }
        }

        /// <inheritdoc cref="Random"/>
        protected override double Sample()
        {
            lock (_sync)
            {
                return base.Sample();
            }
        }

        private static int GetSeed()
        {
            int result;
            byte[] g = Guid.NewGuid().ToByteArray();
            unchecked
            {
                int i0 = BitConverter.ToInt32(g, 0);
                int i1 = BitConverter.ToInt32(g, 4);
                int i2 = BitConverter.ToInt32(g, 8);
                int i3 = BitConverter.ToInt32(g, 12);

                result = i0 + i1 * 3 + i2 * 7 + i3 * 11;
                result = Math.Abs(result);
            }
            return result;
        }
    }
}
