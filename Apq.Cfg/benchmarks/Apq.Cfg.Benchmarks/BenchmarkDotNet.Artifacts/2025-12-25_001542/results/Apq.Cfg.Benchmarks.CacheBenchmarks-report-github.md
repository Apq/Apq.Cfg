```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 6.0 : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 9.0 : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=10  LaunchCount=1  WarmupCount=5  

```
| Method                             | Job      | Runtime  | Mean          | Error        | StdDev       | Ratio           | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------------- |--------- |--------- |--------------:|-------------:|-------------:|----------------:|--------:|-------:|----------:|------------:|
| CacheMiss_NonExistentKey_1000      | .NET 6.0 | .NET 6.0 |  22,230.40 ns |   329.985 ns |   218.265 ns |   1.053x slower |   0.01x |      - |         - |          NA |
| CacheMiss_DifferentNonExistentKeys | .NET 6.0 | .NET 6.0 |   2,629.86 ns |    28.764 ns |    19.026 ns |   8.031x faster |   0.08x |      - |         - |          NA |
| CacheMiss_MixedExistence           | .NET 6.0 | .NET 6.0 |   5,207.94 ns |    39.107 ns |    25.867 ns |   4.055x faster |   0.04x |      - |         - |          NA |
| ColdPath_Sequential_100Keys        | .NET 6.0 | .NET 6.0 |   2,577.08 ns |    32.513 ns |    21.505 ns |   8.196x faster |   0.09x |      - |         - |          NA |
| ColdPath_Random_Pattern            | .NET 6.0 | .NET 6.0 |  26,150.68 ns |   172.154 ns |    90.040 ns |   1.238x slower |   0.01x |      - |     104 B |          NA |
| Exists_SameKey_1000                | .NET 6.0 | .NET 6.0 |  21,289.17 ns |   164.963 ns |   109.113 ns |   1.008x slower |   0.01x |      - |         - |          NA |
| Exists_NonExistentKey_1000         | .NET 6.0 | .NET 6.0 |  22,206.32 ns |   182.944 ns |   121.006 ns |   1.051x slower |   0.01x |      - |         - |          NA |
| Exists_Mixed_1000                  | .NET 6.0 | .NET 6.0 |  21,473.50 ns |   246.283 ns |   146.559 ns |   1.017x slower |   0.01x |      - |         - |          NA |
| FirstAccess_NewKey                 | .NET 6.0 | .NET 6.0 |      23.03 ns |     0.164 ns |     0.108 ns | 916.968x faster |   8.20x |      - |         - |          NA |
| SubsequentAccess_Warmed            | .NET 6.0 | .NET 6.0 |   2,421.19 ns |    14.308 ns |     8.515 ns |   8.723x faster |   0.07x |      - |         - |          NA |
| HotPath_SameKey_1000               | .NET 6.0 | .NET 6.0 |  21,120.16 ns |   289.592 ns |   172.331 ns |        baseline |         |      - |         - |          NA |
| HotPath_SameKey_10000              | .NET 6.0 | .NET 6.0 | 210,376.33 ns | 2,040.642 ns | 1,349.759 ns |   9.962x slower |   0.10x |      - |         - |          NA |
| HotPath_TwoKeys_Alternating        | .NET 6.0 | .NET 6.0 |  44,101.86 ns |   264.027 ns |   157.118 ns |   2.088x slower |   0.02x |      - |         - |          NA |
| HotPath_FewKeys_Loop               | .NET 6.0 | .NET 6.0 | 114,968.39 ns | 1,780.901 ns | 1,177.956 ns |   5.444x slower |   0.07x |      - |         - |          NA |
| WriteAndRead_SameKey               | .NET 6.0 | .NET 6.0 |   5,549.82 ns |    27.066 ns |    17.903 ns |   3.806x faster |   0.03x | 0.0839 |    4000 B |          NA |
| WriteAndRead_DifferentKeys         | .NET 6.0 | .NET 6.0 |   8,886.14 ns |    77.017 ns |    45.832 ns |   2.377x faster |   0.02x | 0.1678 |    8720 B |          NA |
| BatchWrite_ThenBatchRead           | .NET 6.0 | .NET 6.0 |  10,228.12 ns |   138.286 ns |    82.292 ns |   2.065x faster |   0.02x | 0.2747 |   13600 B |          NA |
|                                    |          |          |               |              |              |                 |         |        |           |             |
| CacheMiss_NonExistentKey_1000      | .NET 8.0 | .NET 8.0 |  12,661.81 ns |   151.299 ns |    90.036 ns |   1.019x faster |   0.01x |      - |         - |          NA |
| CacheMiss_DifferentNonExistentKeys | .NET 8.0 | .NET 8.0 |   1,513.18 ns |    26.723 ns |    17.676 ns |   8.527x faster |   0.10x |      - |         - |          NA |
| CacheMiss_MixedExistence           | .NET 8.0 | .NET 8.0 |   3,189.43 ns |    28.144 ns |    16.748 ns |   4.045x faster |   0.03x |      - |         - |          NA |
| ColdPath_Sequential_100Keys        | .NET 8.0 | .NET 8.0 |   1,608.59 ns |    21.336 ns |    14.113 ns |   8.021x faster |   0.08x |      - |         - |          NA |
| ColdPath_Random_Pattern            | .NET 8.0 | .NET 8.0 |  16,298.97 ns |   120.425 ns |    71.663 ns |   1.263x slower |   0.01x |      - |     104 B |          NA |
| Exists_SameKey_1000                | .NET 8.0 | .NET 8.0 |  13,025.61 ns |   308.387 ns |   183.516 ns |   1.010x slower |   0.01x |      - |         - |          NA |
| Exists_NonExistentKey_1000         | .NET 8.0 | .NET 8.0 |  12,597.73 ns |   147.277 ns |    87.642 ns |   1.024x faster |   0.01x |      - |         - |          NA |
| Exists_Mixed_1000                  | .NET 8.0 | .NET 8.0 |  12,771.41 ns |   111.840 ns |    73.975 ns |   1.010x faster |   0.01x |      - |         - |          NA |
| FirstAccess_NewKey                 | .NET 8.0 | .NET 8.0 |      15.22 ns |     0.270 ns |     0.179 ns | 847.613x faster |  10.34x |      - |         - |          NA |
| SubsequentAccess_Warmed            | .NET 8.0 | .NET 8.0 |   1,482.80 ns |    12.593 ns |     8.330 ns |   8.701x faster |   0.06x |      - |         - |          NA |
| HotPath_SameKey_1000               | .NET 8.0 | .NET 8.0 |  12,901.03 ns |   105.172 ns |    69.565 ns |        baseline |         |      - |         - |          NA |
| HotPath_SameKey_10000              | .NET 8.0 | .NET 8.0 | 136,207.60 ns | 1,643.352 ns | 1,086.976 ns |  10.558x slower |   0.10x |      - |         - |          NA |
| HotPath_TwoKeys_Alternating        | .NET 8.0 | .NET 8.0 |  27,630.55 ns |   295.411 ns |   195.396 ns |   2.142x slower |   0.02x |      - |         - |          NA |
| HotPath_FewKeys_Loop               | .NET 8.0 | .NET 8.0 |  73,579.07 ns |   580.300 ns |   383.833 ns |   5.703x slower |   0.04x |      - |         - |          NA |
| WriteAndRead_SameKey               | .NET 8.0 | .NET 8.0 |   4,262.95 ns |    45.973 ns |    30.408 ns |   3.026x faster |   0.03x | 0.0839 |    4000 B |          NA |
| WriteAndRead_DifferentKeys         | .NET 8.0 | .NET 8.0 |   7,590.66 ns |    80.862 ns |    48.120 ns |   1.700x faster |   0.01x | 0.1755 |    8720 B |          NA |
| BatchWrite_ThenBatchRead           | .NET 8.0 | .NET 8.0 |   9,035.62 ns |    87.549 ns |    52.099 ns |   1.428x faster |   0.01x | 0.2747 |   13600 B |          NA |
|                                    |          |          |               |              |              |                 |         |        |           |             |
| CacheMiss_NonExistentKey_1000      | .NET 9.0 | .NET 9.0 |  13,572.08 ns |   103.289 ns |    68.320 ns |   1.010x faster |   0.01x |      - |         - |          NA |
| CacheMiss_DifferentNonExistentKeys | .NET 9.0 | .NET 9.0 |   1,488.12 ns |    10.770 ns |     6.409 ns |   9.208x faster |   0.10x |      - |         - |          NA |
| CacheMiss_MixedExistence           | .NET 9.0 | .NET 9.0 |   3,390.74 ns |    32.173 ns |    21.280 ns |   4.041x faster |   0.05x |      - |         - |          NA |
| ColdPath_Sequential_100Keys        | .NET 9.0 | .NET 9.0 |   1,601.67 ns |    17.711 ns |    11.715 ns |   8.556x faster |   0.10x |      - |         - |          NA |
| ColdPath_Random_Pattern            | .NET 9.0 | .NET 9.0 |  15,467.45 ns |   154.518 ns |   102.204 ns |   1.129x slower |   0.01x |      - |     104 B |          NA |
| Exists_SameKey_1000                | .NET 9.0 | .NET 9.0 |  14,416.80 ns |   191.820 ns |   126.877 ns |   1.052x slower |   0.01x |      - |         - |          NA |
| Exists_NonExistentKey_1000         | .NET 9.0 | .NET 9.0 |  13,586.22 ns |   148.801 ns |    98.423 ns |   1.009x faster |   0.01x |      - |         - |          NA |
| Exists_Mixed_1000                  | .NET 9.0 | .NET 9.0 |  13,769.42 ns |   204.165 ns |   135.042 ns |   1.005x slower |   0.01x |      - |         - |          NA |
| FirstAccess_NewKey                 | .NET 9.0 | .NET 9.0 |      14.71 ns |     0.244 ns |     0.161 ns | 931.500x faster |  13.26x |      - |         - |          NA |
| SubsequentAccess_Warmed            | .NET 9.0 | .NET 9.0 |   1,435.01 ns |    13.021 ns |     8.612 ns |   9.549x faster |   0.11x |      - |         - |          NA |
| HotPath_SameKey_1000               | .NET 9.0 | .NET 9.0 |  13,702.78 ns |   210.580 ns |   139.286 ns |        baseline |         |      - |         - |          NA |
| HotPath_SameKey_10000              | .NET 9.0 | .NET 9.0 | 136,109.88 ns | 1,791.276 ns | 1,184.818 ns |   9.934x slower |   0.13x |      - |         - |          NA |
| HotPath_TwoKeys_Alternating        | .NET 9.0 | .NET 9.0 |  28,366.97 ns |   361.909 ns |   239.380 ns |   2.070x slower |   0.03x |      - |         - |          NA |
| HotPath_FewKeys_Loop               | .NET 9.0 | .NET 9.0 |  74,220.98 ns | 1,129.071 ns |   746.810 ns |   5.417x slower |   0.07x |      - |         - |          NA |
| WriteAndRead_SameKey               | .NET 9.0 | .NET 9.0 |   3,448.97 ns |    25.298 ns |    15.055 ns |   3.973x faster |   0.04x | 0.0801 |    4000 B |          NA |
| WriteAndRead_DifferentKeys         | .NET 9.0 | .NET 9.0 |   6,499.22 ns |   280.783 ns |   185.721 ns |   2.110x faster |   0.06x | 0.1755 |    8720 B |          NA |
| BatchWrite_ThenBatchRead           | .NET 9.0 | .NET 9.0 |   7,334.83 ns |    59.978 ns |    39.672 ns |   1.868x faster |   0.02x | 0.2747 |   13600 B |          NA |
