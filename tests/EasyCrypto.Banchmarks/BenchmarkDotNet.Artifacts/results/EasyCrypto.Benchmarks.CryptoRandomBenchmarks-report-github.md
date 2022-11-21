``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=6.0.402
  [Host]     : .NET Core 3.1.30 (CoreCLR 4.700.22.47601, CoreFX 4.700.22.47602), X64 RyuJIT
  DefaultJob : .NET Core 3.1.30 (CoreCLR 4.700.22.47601, CoreFX 4.700.22.47602), X64 RyuJIT


```
|              Method |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
|   RandomIntNoBuffer | 46.92 ns | 0.362 ns | 0.339 ns | 0.0038 |     - |     - |      64 B |
| RandomIntWithBuffer | 36.22 ns | 0.573 ns | 0.478 ns | 0.0066 |     - |     - |     111 B |
