``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100-rc.2.20479.15
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.47505, CoreFX 5.0.20.47505), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.47505, CoreFX 5.0.20.47505), X64 RyuJIT


```
| Method |     Mean |   Error |  StdDev |       Gen 0 |      Gen 1 |     Gen 2 | Allocated |
|------- |---------:|--------:|--------:|------------:|-----------:|----------:|----------:|
|    One | 228.2 ms | 3.42 ms | 4.20 ms | 124000.0000 | 10500.0000 | 1000.0000 | 530.46 MB |
