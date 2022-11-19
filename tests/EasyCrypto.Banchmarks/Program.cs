using BenchmarkDotNet.Running;

namespace EasyCrypto.Benchmarks
{
    static class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<CryptoRandomBenchmarks>();
        }
    }
}
