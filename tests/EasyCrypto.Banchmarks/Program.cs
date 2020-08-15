using BenchmarkDotNet.Running;
using System;

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
