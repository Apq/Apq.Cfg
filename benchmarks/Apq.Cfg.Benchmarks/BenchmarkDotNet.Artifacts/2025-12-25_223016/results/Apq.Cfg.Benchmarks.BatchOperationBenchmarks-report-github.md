```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 6.0 : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 9.0 : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Mean       | Error     | StdDev    |
|-----------:|----------:|----------:|
|   476.7 ns |  68.35 ns |  45.21 ns |
|   292.2 ns |  23.86 ns |  14.20 ns |
| 2,398.1 ns | 140.33 ns |  73.39 ns |
| 1,422.6 ns |  63.21 ns |  37.61 ns |
| 5,087.4 ns | 294.52 ns | 194.81 ns |
| 2,870.5 ns | 219.32 ns | 130.52 ns |
|   539.4 ns |  29.38 ns |  17.48 ns |
|         NA |        NA |        NA |
| 1,264.0 ns |  94.78 ns |  62.69 ns |
|   931.1 ns |  65.82 ns |  39.17 ns |
|   925.1 ns |  49.83 ns |  29.65 ns |
|   729.2 ns |  80.50 ns |  53.24 ns |
| 4,645.9 ns | 469.25 ns | 310.38 ns |
| 3,643.5 ns | 165.11 ns |  98.26 ns |
| 9,176.6 ns | 368.23 ns | 192.59 ns |
| 7,355.8 ns | 336.00 ns | 199.95 ns |
|            |           |           |
|   351.4 ns |  11.23 ns |   6.69 ns |
|   167.6 ns |   3.83 ns |   2.00 ns |
| 1,694.7 ns |  98.12 ns |  64.90 ns |
|   897.3 ns |  47.83 ns |  28.46 ns |
| 3,594.1 ns | 166.00 ns |  98.78 ns |
| 1,854.3 ns |  74.43 ns |  44.29 ns |
|   385.4 ns |  24.08 ns |  14.33 ns |
|         NA |        NA |        NA |
|   985.6 ns |  66.07 ns |  39.32 ns |
|   665.4 ns |  32.25 ns |  19.19 ns |
|   933.2 ns |  24.91 ns |  14.83 ns |
|   688.1 ns |  28.70 ns |  18.98 ns |
| 4,839.3 ns | 338.51 ns | 223.90 ns |
| 3,300.5 ns | 167.13 ns |  99.46 ns |
| 9,364.1 ns | 561.92 ns | 371.67 ns |
| 7,065.6 ns | 394.35 ns | 260.84 ns |
|            |           |           |
|   346.2 ns |  22.98 ns |  15.20 ns |
|   166.9 ns |   7.59 ns |   4.52 ns |
| 1,595.0 ns | 103.92 ns |  61.84 ns |
|   854.5 ns |  14.30 ns |   7.48 ns |
| 3,474.2 ns | 308.38 ns | 203.97 ns |
| 1,833.0 ns | 105.84 ns |  62.98 ns |
|   373.1 ns |  17.35 ns |  10.33 ns |
|         NA |        NA |        NA |
|   952.4 ns |  34.02 ns |  20.24 ns |
|   664.1 ns |  55.23 ns |  36.53 ns |
|   797.3 ns |  38.66 ns |  23.01 ns |
|   560.3 ns |  17.68 ns |  10.52 ns |
| 4,094.2 ns | 160.57 ns | 106.21 ns |
| 2,655.2 ns | 132.16 ns |  78.65 ns |
| 7,751.9 ns | 206.82 ns | 123.08 ns |
| 5,408.5 ns | 116.54 ns |  60.95 ns |

Benchmarks with issues:
  BatchOperationBenchmarks.Get_Typed_Loop_10Keys: .NET 6.0(Runtime=.NET 6.0, IterationCount=10, LaunchCount=1, WarmupCount=5)
  BatchOperationBenchmarks.Get_Typed_Loop_10Keys: .NET 8.0(Runtime=.NET 8.0, IterationCount=10, LaunchCount=1, WarmupCount=5)
  BatchOperationBenchmarks.Get_Typed_Loop_10Keys: .NET 9.0(Runtime=.NET 9.0, IterationCount=10, LaunchCount=1, WarmupCount=5)
