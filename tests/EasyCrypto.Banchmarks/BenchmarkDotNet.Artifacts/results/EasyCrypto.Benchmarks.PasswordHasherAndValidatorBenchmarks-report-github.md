``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22621
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=6.0.402
  [Host]     : .NET Core 3.1.30 (CoreCLR 4.700.22.47601, CoreFX 4.700.22.47602), X64 RyuJIT
  DefaultJob : .NET Core 3.1.30 (CoreCLR 4.700.22.47601, CoreFX 4.700.22.47602), X64 RyuJIT


```
|                      Method |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------- |---------:|---------:|---------:|------:|------:|------:|----------:|
|              UsingOldHasher | 11.88 ms | 0.030 ms | 0.026 ms |     - |     - |     - |   1.23 KB |
|              UsingNewHasher | 26.84 ms | 0.109 ms | 0.091 ms |     - |     - |     - |   2.22 KB |
| UsingNewHasher56KIterations | 53.74 ms | 0.299 ms | 0.249 ms |     - |     - |     - |   2.26 KB |
