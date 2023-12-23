```

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22621.2861/22H2/2022Update/SunValley2)
AMD Ryzen 5 5600X, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method          | Mean          | Error      | StdDev     |
|---------------- |--------------:|-----------:|-----------:|
| AesEncrypt      | 11,780.431 μs | 26.9828 μs | 25.2397 μs |
| AesDecrypt      | 23,168.337 μs | 52.8699 μs | 46.8678 μs |
| AesEncryptQuick |      3.837 μs |  0.0280 μs |  0.0248 μs |
| AesDecryptQuick |      4.181 μs |  0.0331 μs |  0.0310 μs |
