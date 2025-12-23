```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]     : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XWHYYA : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OTNOSO : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-GRHDSK : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2


```
| Method                        | Runtime  | ThreadCount | Mean        | Error     | StdDev     | Median      | Ratio | RatioSD | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |--------- |------------ |------------:|----------:|-----------:|------------:|------:|--------:|-----:|-------:|-------:|----------:|------------:|
| ConcurrentRead_DifferentKeys  | .NET 9.0 | 1           |    37.38 μs |  0.425 μs |   0.397 μs |    37.30 μs |  0.72 |    0.01 |    1 | 0.7324 |      - |  35.59 KB |        0.85 |
| ConcurrentRead_SameKey        | .NET 9.0 | 1           |    38.09 μs |  0.614 μs |   0.513 μs |    38.06 μs |  0.74 |    0.01 |    1 | 0.7324 |      - |  35.44 KB |        0.85 |
| ConcurrentWrite_DifferentKeys | .NET 9.0 | 1           |    39.08 μs |  0.417 μs |   0.348 μs |    39.11 μs |  0.75 |    0.01 |    1 | 0.4883 |      - |  23.67 KB |        0.57 |
| ConcurrentWrite_DifferentKeys | .NET 8.0 | 1           |    42.40 μs |  0.271 μs |   0.254 μs |    42.42 μs |  0.82 |    0.01 |    2 | 0.4883 |      - |  23.67 KB |        0.57 |
| ConcurrentRead_SameKey        | .NET 8.0 | 1           |    45.83 μs |  0.497 μs |   0.441 μs |    45.90 μs |  0.88 |    0.02 |    3 | 0.7935 |      - |  38.56 KB |        0.93 |
| ConcurrentWrite_DifferentKeys | .NET 6.0 | 1           |    45.96 μs |  0.329 μs |   0.257 μs |    45.99 μs |  0.89 |    0.01 |    3 | 0.5493 |      - |  27.58 KB |        0.66 |
| ConcurrentRead_DifferentKeys  | .NET 8.0 | 1           |    46.73 μs |  0.802 μs |   0.711 μs |    46.61 μs |  0.90 |    0.02 |    3 | 0.7935 |      - |  38.71 KB |        0.93 |
| ConcurrentRead_SameKey        | .NET 6.0 | 1           |    51.83 μs |  0.865 μs |   0.809 μs |    51.61 μs |  1.00 |    0.02 |    4 | 0.7935 |      - |  41.69 KB |        1.00 |
| ConcurrentRead_DifferentKeys  | .NET 6.0 | 1           |    53.80 μs |  0.955 μs |   0.893 μs |    53.64 μs |  1.04 |    0.02 |    4 | 0.8545 |      - |  41.84 KB |        1.00 |
| ConcurrentExists              | .NET 8.0 | 1           |    64.00 μs |  0.759 μs |   0.710 μs |    63.94 μs |  1.23 |    0.02 |    5 | 0.3662 |      - |  17.47 KB |        0.42 |
| ConcurrentMixed_ReadWrite     | .NET 9.0 | 1           |    66.44 μs |  0.714 μs |   0.668 μs |    66.52 μs |  1.28 |    0.02 |    5 | 0.8545 |      - |  44.06 KB |        1.06 |
| ConcurrentMixed_ReadWrite     | .NET 8.0 | 1           |    67.78 μs |  0.941 μs |   0.881 μs |    67.44 μs |  1.31 |    0.03 |    5 | 0.9766 |      - |  47.19 KB |        1.13 |
| ConcurrentExists              | .NET 9.0 | 1           |    72.06 μs |  1.420 μs |   1.459 μs |    71.68 μs |  1.39 |    0.03 |    6 | 0.3662 |      - |  17.47 KB |        0.42 |
| ConcurrentMixed_ReadWrite     | .NET 6.0 | 1           |    75.86 μs |  1.487 μs |   1.934 μs |    75.72 μs |  1.46 |    0.04 |    6 | 1.0986 |      - |  54.22 KB |        1.30 |
| ConcurrentExists              | .NET 6.0 | 1           |    78.13 μs |  1.968 μs |   5.802 μs |    80.75 μs |  1.51 |    0.11 |    6 | 0.4883 |      - |  25.28 KB |        0.61 |
|                               |          |             |             |           |            |             |       |         |      |        |        |           |             |
| ConcurrentWrite_DifferentKeys | .NET 6.0 | 4           |   196.15 μs |  3.891 μs |   9.247 μs |   195.85 μs |  0.97 |    0.07 |    1 | 2.1973 |      - | 109.89 KB |        0.66 |
| ConcurrentRead_DifferentKeys  | .NET 6.0 | 4           |   198.02 μs |  3.930 μs |   8.789 μs |   196.67 μs |  0.98 |    0.07 |    1 | 3.4180 |      - | 166.57 KB |        1.00 |
| ConcurrentRead_SameKey        | .NET 6.0 | 4           |   202.85 μs |  4.054 μs |  11.166 μs |   202.63 μs |  1.00 |    0.08 |    1 | 3.4180 |      - | 166.33 KB |        1.00 |
| ConcurrentRead_DifferentKeys  | .NET 8.0 | 4           |   215.08 μs |  6.343 μs |  18.702 μs |   216.32 μs |  1.06 |    0.11 |    1 | 3.1738 |      - | 154.07 KB |        0.93 |
| ConcurrentRead_SameKey        | .NET 9.0 | 4           |   216.08 μs |  6.376 μs |  18.799 μs |   217.90 μs |  1.07 |    0.11 |    1 | 2.9297 |      - | 141.33 KB |        0.85 |
| ConcurrentWrite_DifferentKeys | .NET 8.0 | 4           |   218.88 μs |  4.365 μs |   6.923 μs |   217.94 μs |  1.08 |    0.07 |    1 | 1.9531 |      - |  94.27 KB |        0.57 |
| ConcurrentRead_SameKey        | .NET 8.0 | 4           |   221.75 μs |  4.383 μs |   9.150 μs |   219.11 μs |  1.10 |    0.07 |    1 | 3.1738 |      - | 153.83 KB |        0.92 |
| ConcurrentWrite_DifferentKeys | .NET 9.0 | 4           |   233.81 μs |  4.639 μs |   9.786 μs |   232.45 μs |  1.16 |    0.08 |    1 | 1.9531 |      - |  94.27 KB |        0.57 |
| ConcurrentRead_DifferentKeys  | .NET 9.0 | 4           |   240.04 μs |  7.212 μs |  21.152 μs |   237.31 μs |  1.19 |    0.12 |    1 | 2.9297 |      - | 141.57 KB |        0.85 |
| ConcurrentMixed_ReadWrite     | .NET 9.0 | 4           |   318.65 μs |  9.182 μs |  26.929 μs |   313.85 μs |  1.58 |    0.16 |    2 | 2.9297 |      - | 135.05 KB |        0.81 |
| ConcurrentMixed_ReadWrite     | .NET 8.0 | 4           |   319.34 μs |  9.578 μs |  28.242 μs |   325.06 μs |  1.58 |    0.16 |    2 | 2.9297 |      - |  141.3 KB |        0.85 |
| ConcurrentExists              | .NET 9.0 | 4           |   394.26 μs | 12.334 μs |  36.174 μs |   394.01 μs |  1.95 |    0.21 |    3 | 1.4648 |      - |  69.45 KB |        0.42 |
| ConcurrentMixed_ReadWrite     | .NET 6.0 | 4           |   400.76 μs | 17.893 μs |  52.757 μs |   401.23 μs |  1.98 |    0.28 |    3 | 3.4180 |      - | 163.17 KB |        0.98 |
| ConcurrentExists              | .NET 8.0 | 4           |   403.91 μs | 10.006 μs |  29.187 μs |   399.40 μs |  2.00 |    0.18 |    3 | 1.4648 |      - |  69.45 KB |        0.42 |
| ConcurrentExists              | .NET 6.0 | 4           |   519.65 μs | 20.075 μs |  59.192 μs |   527.44 μs |  2.57 |    0.32 |    4 | 1.9531 |      - |  100.7 KB |        0.61 |
|                               |          |             |             |           |            |             |       |         |      |        |        |           |             |
| ConcurrentRead_SameKey        | .NET 8.0 | 8           |   399.25 μs |  7.938 μs |  14.714 μs |   399.55 μs |  0.90 |    0.06 |    1 | 5.8594 |      - | 307.52 KB |        0.92 |
| ConcurrentWrite_DifferentKeys | .NET 6.0 | 8           |   421.98 μs |  8.150 μs |  11.156 μs |   421.63 μs |  0.95 |    0.06 |    2 | 4.8828 | 0.4883 | 219.65 KB |        0.66 |
| ConcurrentWrite_DifferentKeys | .NET 9.0 | 8           |   438.68 μs |  8.664 μs |  16.059 μs |   436.31 μs |  0.99 |    0.07 |    2 | 3.9063 |      - | 188.39 KB |        0.57 |
| ConcurrentRead_SameKey        | .NET 6.0 | 8           |   445.81 μs |  9.113 μs |  26.582 μs |   442.98 μs |  1.00 |    0.08 |    2 | 5.8594 |      - | 332.52 KB |        1.00 |
| ConcurrentWrite_DifferentKeys | .NET 8.0 | 8           |   451.72 μs |  8.002 μs |   7.485 μs |   451.53 μs |  1.02 |    0.06 |    2 | 3.9063 |      - | 188.39 KB |        0.57 |
| ConcurrentRead_SameKey        | .NET 9.0 | 8           |   454.89 μs |  9.079 μs |  25.157 μs |   446.09 μs |  1.02 |    0.08 |    2 | 5.8594 |      - | 282.52 KB |        0.85 |
| ConcurrentRead_DifferentKeys  | .NET 8.0 | 8           |   464.90 μs |  9.239 μs |  25.908 μs |   466.12 μs |  1.05 |    0.08 |    2 | 6.3477 |      - | 307.88 KB |        0.93 |
| ConcurrentRead_DifferentKeys  | .NET 9.0 | 8           |   512.97 μs | 27.026 μs |  79.687 μs |   521.97 μs |  1.15 |    0.19 |    3 | 5.8594 |      - | 282.88 KB |        0.85 |
| ConcurrentRead_DifferentKeys  | .NET 6.0 | 8           |   564.67 μs | 11.209 μs |  30.304 μs |   559.74 μs |  1.27 |    0.10 |    3 | 5.8594 |      - | 332.88 KB |        1.00 |
| ConcurrentMixed_ReadWrite     | .NET 9.0 | 8           |   645.10 μs | 12.829 μs |  28.429 μs |   643.08 μs |  1.45 |    0.11 |    4 | 4.8828 |      - | 269.96 KB |        0.81 |
| ConcurrentMixed_ReadWrite     | .NET 8.0 | 8           |   778.88 μs | 15.965 μs |  46.823 μs |   775.76 μs |  1.75 |    0.15 |    5 | 4.8828 |      - | 282.45 KB |        0.85 |
| ConcurrentExists              | .NET 6.0 | 8           |   884.68 μs | 35.116 μs | 101.877 μs |   860.52 μs |  1.99 |    0.26 |    6 | 3.9063 |      - | 201.27 KB |        0.61 |
| ConcurrentMixed_ReadWrite     | .NET 6.0 | 8           |   895.64 μs | 23.906 μs |  70.113 μs |   901.38 μs |  2.02 |    0.20 |    6 | 6.8359 |      - | 326.21 KB |        0.98 |
| ConcurrentExists              | .NET 9.0 | 8           |   924.46 μs | 25.908 μs |  76.391 μs |   904.73 μs |  2.08 |    0.21 |    6 | 2.9297 |      - | 138.77 KB |        0.42 |
| ConcurrentExists              | .NET 8.0 | 8           | 1,030.20 μs | 35.829 μs | 103.375 μs | 1,057.72 μs |  2.32 |    0.27 |    7 | 3.9063 |      - | 138.77 KB |        0.42 |
