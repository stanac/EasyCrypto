using BenchmarkDotNet.Running;

namespace EasyCrypto.Banchmarks
{
    static class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<CryptoRandomBanchmarks>();
        }
    }
}
