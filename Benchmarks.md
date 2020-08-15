// * Summary *

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1016 (1909/November2018Update/19H2)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.100-preview.7.20366.6
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT

|              Method |     Mean |    Error |   StdDev |
|-------------------- |---------:|---------:|---------:|
|   RandomIntNoBuffer | 80.37 ns | 1.498 ns | 1.251 ns |
| RandomIntWithBuffer | 60.88 ns | 1.064 ns | 0.995 ns |

// * Hints *
Outliers
  CryptoRandomBanchmarks.RandomIntNoBuffer: Default -> 3 outliers were removed (88.08 ns..91.41 ns)

// * Legends *

 - Mean   : Arithmetic mean of all measurements
 - Error  : Half of 99.9% confidence interval
 - StdDev : Standard deviation of all measurements
 - 1 ns   : 1 Nanosecond (0.000000001 sec)