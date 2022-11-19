using BenchmarkDotNet.Attributes;

namespace EasyCrypto.Benchmarks
{
    [MemoryDiagnoser]
    public class CryptoRandomBenchmarks
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
