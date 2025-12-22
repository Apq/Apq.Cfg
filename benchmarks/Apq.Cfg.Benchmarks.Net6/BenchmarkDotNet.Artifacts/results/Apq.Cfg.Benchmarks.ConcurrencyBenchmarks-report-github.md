```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]     : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2


```
| Method                        | ThreadCount | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |------------ |----------:|----------:|----------:|----------:|------:|--------:|-----:|-------:|-------:|----------:|------------:|
| ConcurrentExists              | 1           |  4.485 μs | 0.0539 μs | 0.0504 μs |  4.483 μs |  0.63 |    0.01 |    1 | 0.0076 |      - |     264 B |       0.008 |
| ConcurrentRead_SameKey        | 1           |  7.123 μs | 0.0767 μs | 0.0680 μs |  7.130 μs |  1.00 |    0.01 |    2 | 0.6790 |      - |   33865 B |       1.000 |
| ConcurrentRead_DifferentKeys  | 1           |  7.383 μs | 0.1438 μs | 0.1345 μs |  7.369 μs |  1.04 |    0.02 |    2 | 0.6790 |      - |   34018 B |       1.005 |
| ConcurrentMixed_ReadWrite     | 1           | 13.091 μs | 0.2464 μs | 0.5034 μs | 12.939 μs |  1.84 |    0.07 |    3 | 0.6714 |      - |   33917 B |       1.002 |
| ConcurrentWrite_DifferentKeys | 1           | 21.483 μs | 0.8206 μs | 2.4066 μs | 22.179 μs |  3.02 |    0.34 |    4 | 0.3662 |      - |   19440 B |       0.574 |
|                               |             |           |           |           |           |       |         |      |        |        |           |             |
| ConcurrentWrite_DifferentKeys | 4           |        NA |        NA |        NA |        NA |     ? |       ? |    ? |     NA |     NA |        NA |           ? |
| ConcurrentExists              | 4           |  9.583 μs | 0.1812 μs | 0.1607 μs |  9.526 μs |  0.45 |    0.02 |    1 | 0.0153 |      - |     701 B |       0.005 |
| ConcurrentRead_SameKey        | 4           | 21.110 μs | 0.4175 μs | 0.7201 μs | 21.068 μs |  1.00 |    0.05 |    2 | 2.6855 |      - |  135117 B |       1.000 |
| ConcurrentRead_DifferentKeys  | 4           | 24.654 μs | 0.4923 μs | 1.0805 μs | 24.378 μs |  1.17 |    0.06 |    3 | 2.6855 |      - |  135368 B |       1.002 |
| ConcurrentMixed_ReadWrite     | 4           | 27.694 μs | 0.5538 μs | 0.5180 μs | 27.794 μs |  1.31 |    0.05 |    4 | 2.1667 | 0.0610 |  106288 B |       0.787 |
|                               |             |           |           |           |           |       |         |      |        |        |           |             |
| ConcurrentMixed_ReadWrite     | 8           |        NA |        NA |        NA |        NA |     ? |       ? |    ? |     NA |     NA |        NA |           ? |
| ConcurrentWrite_DifferentKeys | 8           |        NA |        NA |        NA |        NA |     ? |       ? |    ? |     NA |     NA |        NA |           ? |
| ConcurrentExists              | 8           | 14.692 μs | 0.1873 μs | 0.1661 μs | 14.733 μs |  0.50 |    0.02 |    1 | 0.0305 |      - |    1276 B |       0.005 |
| ConcurrentRead_SameKey        | 8           | 29.689 μs | 0.5841 μs | 1.2699 μs | 29.603 μs |  1.00 |    0.06 |    2 | 5.4016 | 0.0305 |  270096 B |       1.000 |
| ConcurrentRead_DifferentKeys  | 8           | 29.830 μs | 0.5959 μs | 1.1763 μs | 29.618 μs |  1.01 |    0.06 |    2 | 5.4016 | 0.0305 |  270471 B |       1.001 |

Benchmarks with issues:
  ConcurrencyBenchmarks.ConcurrentWrite_DifferentKeys: DefaultJob [ThreadCount=4]
  ConcurrencyBenchmarks.ConcurrentMixed_ReadWrite: DefaultJob [ThreadCount=8]
  ConcurrencyBenchmarks.ConcurrentWrite_DifferentKeys: DefaultJob [ThreadCount=8]
