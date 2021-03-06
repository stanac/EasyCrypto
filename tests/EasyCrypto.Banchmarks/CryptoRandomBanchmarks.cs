﻿using BenchmarkDotNet.Attributes;

namespace EasyCrypto.Banchmarks
{
    public class CryptoRandomBanchmarks
    {
        private readonly CryptoRandom crWithBuffer = new CryptoRandom(true);
        private readonly CryptoRandom crWithoutBuffer = new CryptoRandom(false);

        [Benchmark]
        public int RandomIntNoBuffer()
        {
            return crWithoutBuffer.NextInt();
        }

        [Benchmark]
        public int RandomIntWithBuffer()
        {
            return crWithBuffer.NextInt();
        }
    }
}
