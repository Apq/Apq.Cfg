```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 6.0 : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 9.0 : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Mean         | Error        | StdDev       |
|-------------:|-------------:|-------------:|
|     72.76 ns |     8.538 ns |     5.648 ns |
|     77.17 ns |     9.123 ns |     6.034 ns |
|     68.24 ns |     6.293 ns |     4.162 ns |
| 16,822.26 ns | 2,439.784 ns | 1,613.766 ns |
|     72.95 ns |     7.277 ns |     4.813 ns |
|     72.46 ns |     5.185 ns |     3.085 ns |
|     50.07 ns |     5.171 ns |     3.420 ns |
| 25,097.66 ns | 2,370.708 ns | 1,568.076 ns |
|    154.99 ns |     7.333 ns |     3.835 ns |
| 16,164.10 ns | 1,261.123 ns |   750.474 ns |
|           NA |           NA |           NA |
|           NA |           NA |           NA |
|              |              |              |
|     47.24 ns |     5.739 ns |     3.796 ns |
|     56.04 ns |     6.078 ns |     4.020 ns |
|     38.80 ns |     2.300 ns |     1.203 ns |
| 10,562.37 ns | 1,681.403 ns | 1,112.144 ns |
|     44.36 ns |     5.770 ns |     3.434 ns |
|     52.82 ns |     3.969 ns |     2.362 ns |
|     29.97 ns |     3.109 ns |     2.056 ns |
| 15,292.98 ns | 1,860.050 ns | 1,230.308 ns |
|    100.80 ns |     3.895 ns |     2.037 ns |
| 10,356.65 ns |   993.955 ns |   591.487 ns |
|           NA |           NA |           NA |
|           NA |           NA |           NA |
|              |              |              |
|     44.77 ns |     2.518 ns |     1.498 ns |
|     48.10 ns |     5.661 ns |     3.744 ns |
|     45.62 ns |     5.232 ns |     3.461 ns |
| 11,108.27 ns | 1,454.882 ns |   962.314 ns |
|     48.50 ns |     7.552 ns |     4.995 ns |
|     47.51 ns |     6.191 ns |     4.095 ns |
|     28.28 ns |     1.861 ns |     1.231 ns |
| 14,414.42 ns | 1,897.436 ns | 1,255.036 ns |
|    107.60 ns |     8.244 ns |     4.906 ns |
| 10,384.42 ns | 1,197.399 ns |   792.006 ns |
|           NA |           NA |           NA |
|           NA |           NA |           NA |

Benchmarks with issues:
  MultiSourceBenchmarks.Write_NewKey: .NET 6.0(Runtime=.NET 6.0, IterationCount=10, LaunchCount=1, WarmupCount=5) [SourceCount=3]
  MultiSourceBenchmarks.Write_OverrideKey: .NET 6.0(Runtime=.NET 6.0, IterationCount=10, LaunchCount=1, WarmupCount=5) [SourceCount=3]
  MultiSourceBenchmarks.Write_NewKey: .NET 8.0(Runtime=.NET 8.0, IterationCount=10, LaunchCount=1, WarmupCount=5) [SourceCount=3]
  MultiSourceBenchmarks.Write_OverrideKey: .NET 8.0(Runtime=.NET 8.0, IterationCount=10, LaunchCount=1, WarmupCount=5) [SourceCount=3]
  MultiSourceBenchmarks.Write_NewKey: .NET 9.0(Runtime=.NET 9.0, IterationCount=10, LaunchCount=1, WarmupCount=5) [SourceCount=3]
  MultiSourceBenchmarks.Write_OverrideKey: .NET 9.0(Runtime=.NET 9.0, IterationCount=10, LaunchCount=1, WarmupCount=5) [SourceCount=3]
