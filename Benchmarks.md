// * Summary *

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22621.2861/22H2/2022Update/SunValley2)
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


| Method          | Mean          | Error      | StdDev     |
|---------------- |--------------:|-----------:|-----------:|
| AesEncrypt      | 11,780.431 us | 26.9828 us | 25.2397 us |
| AesDecrypt      | 23,168.337 us | 52.8699 us | 46.8678 us |
| AesEncryptQuick |      3.837 us |  0.0280 us |  0.0248 us |
| AesDecryptQuick |      4.181 us |  0.0331 us |  0.0310 us |

// * Hints *
Outliers
  EncryptionBenchmark.AesDecrypt: Default      -> 1 outlier  was  removed (23.32 ms)
  EncryptionBenchmark.AesEncryptQuick: Default -> 1 outlier  was  removed (3.92 us)

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 us   : 1 Microsecond (0.000001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:01:17 (77.51 sec), executed benchmarks: 4