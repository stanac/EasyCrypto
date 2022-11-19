using System;

namespace EasyCrypto
{
    internal class SlidingBuffer
    {
        private readonly object _sync = new object();
        private readonly Action<byte[]> _generateData;
        private byte[] _cache = Array.Empty<byte>();

        public SlidingBuffer(Action<byte[]> generateData)
        {
            _generateData = generateData ?? throw new ArgumentNullException(nameof(generateData));
        }

        public void GetData(byte[] data)
        {
            lock (_sync)
            {
                if (_cache.Length < data.Length)
                {
                    byte[] temp = new byte[data.Length * 8];
                    _generateData(temp);

                    Buffer.BlockCopy(_cache, 0, data, 0, _cache.Length);
                    Buffer.BlockCopy(temp, 0, data, _cache.Length, data.Length - _cache.Length);
                    byte[] cache = new byte[temp.Length + _cache.Length - data.Length];
                    Buffer.BlockCopy(temp, data.Length - _cache.Length, cache, 0, cache.Length);
                    _cache = cache;
                }
                else
                {
                    Buffer.BlockCopy(_cache, 0, data, 0, data.Length);
                    byte[] cache = new byte[_cache.Length - data.Length];
                    Buffer.BlockCopy(_cache, data.Length, cache, 0, cache.Length);
                    _cache = cache;
                }
            }
        }
    }
}
