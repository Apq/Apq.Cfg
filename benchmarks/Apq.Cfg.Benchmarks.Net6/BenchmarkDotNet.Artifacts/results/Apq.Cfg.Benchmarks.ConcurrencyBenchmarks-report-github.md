```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]     : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2


```
| Method                        | ThreadCount | Mean      | Error     | StdDev    | Ratio | RatioSD | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |------------ |----------:|----------:|----------:|------:|--------:|-----:|-------:|-------:|----------:|------------:|
| ConcurrentRead_DifferentKeys  | 1           |  40.34 μs |  0.262 μs |  0.219 μs |  0.99 |    0.01 |    1 | 0.7935 |      - |  41.84 KB |        1.00 |
| ConcurrentRead_SameKey        | 1           |  40.56 μs |  0.407 μs |  0.381 μs |  1.00 |    0.01 |    1 | 0.7935 |      - |  41.69 KB |        1.00 |
| ConcurrentWrite_DifferentKeys | 1           |  43.09 μs |  0.186 μs |  0.165 μs |  1.06 |    0.01 |    2 | 0.5493 |      - |  27.58 KB |        0.66 |
| ConcurrentExists              | 1           |  65.26 μs |  0.271 μs |  0.254 μs |  1.61 |    0.02 |    3 | 0.4883 |      - |  25.28 KB |        0.61 |
| ConcurrentMixed_ReadWrite     | 1           |  68.90 μs |  0.384 μs |  0.359 μs |  1.70 |    0.02 |    4 | 1.0986 |      - |  54.22 KB |        1.30 |
|                               |             |           |           |           |       |         |      |        |        |           |             |
| ConcurrentRead_SameKey        | 4           | 151.47 μs |  2.449 μs |  2.291 μs |  1.00 |    0.02 |    1 | 3.1738 |      - | 166.33 KB |        1.00 |
| ConcurrentRead_DifferentKeys  | 4           | 154.17 μs |  1.627 μs |  1.442 μs |  1.02 |    0.02 |    1 | 3.1738 |      - | 166.57 KB |        1.00 |
| ConcurrentWrite_DifferentKeys | 4           | 162.41 μs |  3.032 μs |  2.836 μs |  1.07 |    0.02 |    2 | 2.1973 |      - | 109.89 KB |        0.66 |
| ConcurrentMixed_ReadWrite     | 4           | 236.35 μs |  2.292 μs |  2.144 μs |  1.56 |    0.03 |    3 | 3.1738 |      - | 163.17 KB |        0.98 |
| ConcurrentExists              | 4           | 282.79 μs |  3.633 μs |  3.221 μs |  1.87 |    0.03 |    4 | 1.9531 |      - |  100.7 KB |        0.61 |
|                               |             |           |           |           |       |         |      |        |        |           |             |
| ConcurrentRead_SameKey        | 8           | 326.42 μs |  6.266 μs |  7.216 μs |  1.00 |    0.03 |    1 | 6.3477 |      - | 332.52 KB |        1.00 |
| ConcurrentRead_DifferentKeys  | 8           | 331.57 μs |  6.428 μs |  6.878 μs |  1.02 |    0.03 |    1 | 6.3477 |      - | 332.88 KB |        1.00 |
| ConcurrentWrite_DifferentKeys | 8           | 347.59 μs |  5.008 μs |  4.684 μs |  1.07 |    0.03 |    1 | 4.3945 | 0.4883 | 219.64 KB |        0.66 |
| ConcurrentMixed_ReadWrite     | 8           | 517.00 μs | 10.236 μs | 16.529 μs |  1.58 |    0.06 |    2 | 5.8594 |      - |  326.2 KB |        0.98 |
| ConcurrentExists              | 8           | 618.13 μs | 12.311 μs | 13.173 μs |  1.89 |    0.06 |    3 | 3.9063 |      - | 201.27 KB |        0.61 |
