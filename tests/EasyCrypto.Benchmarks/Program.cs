using BenchmarkDotNet.Running;

namespace EasyCrypto.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var s = BenchmarkRunner.Run<EncryptionBenchmark>();

            return;

            var t = new EncryptionBenchmark();

            t.AesDecrypt();
            t.AesEncrypt();
            t.AesDecryptQuick();
            t.AesEncryptQuick();
        }
    }
}
