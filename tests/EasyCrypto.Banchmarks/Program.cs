using BenchmarkDotNet.Running;
using System;

namespace EasyCrypto.Banchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CryptoRandomBanchmarks>();
        }
    }
}
