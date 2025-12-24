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
| Method              | Job      | Runtime  | Mean          | Error        | StdDev       | Ratio            | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------------- |--------- |--------- |--------------:|-------------:|-------------:|-----------------:|--------:|-------:|-------:|----------:|------------:|
| Get_String          | .NET 6.0 | .NET 6.0 |      24.70 ns |     0.135 ns |     0.080 ns |         baseline |         |      - |      - |         - |          NA |
| Get_String_Generic  | .NET 6.0 | .NET 6.0 |      88.60 ns |     1.311 ns |     0.867 ns |     3.59x slower |   0.04x | 0.0017 | 0.0001 |      80 B |          NA |
| Get_Int             | .NET 6.0 | .NET 6.0 |     142.95 ns |     7.469 ns |     4.940 ns |     5.79x slower |   0.19x | 0.0025 |      - |     128 B |          NA |
| Get_Long            | .NET 6.0 | .NET 6.0 |     126.10 ns |     4.635 ns |     3.066 ns |     5.11x slower |   0.12x | 0.0026 |      - |     128 B |          NA |
| Get_Double          | .NET 6.0 | .NET 6.0 |     140.42 ns |     2.351 ns |     1.555 ns |     5.69x slower |   0.06x | 0.0026 |      - |     128 B |          NA |
| Get_Decimal         | .NET 6.0 | .NET 6.0 |     140.98 ns |     2.253 ns |     1.490 ns |     5.71x slower |   0.06x | 0.0029 |      - |     144 B |          NA |
| Get_Bool            | .NET 6.0 | .NET 6.0 |     107.46 ns |     2.197 ns |     1.453 ns |     4.35x slower |   0.06x | 0.0025 |      - |     128 B |          NA |
| Get_Int_Multiple    | .NET 6.0 | .NET 6.0 | 120,271.64 ns | 1,383.445 ns |   915.063 ns | 4,870.20x slower |  38.40x | 2.5635 |      - |  128000 B |          NA |
| Get_Bool_Multiple   | .NET 6.0 | .NET 6.0 | 106,232.63 ns |   696.343 ns |   460.588 ns | 4,301.71x slower |  22.19x | 2.5635 |      - |  128000 B |          NA |
| Get_Double_Multiple | .NET 6.0 | .NET 6.0 | 148,388.26 ns | 3,005.774 ns | 1,988.134 ns | 6,008.73x slower |  79.00x | 2.6855 |      - |  128000 B |          NA |
| Get_String_Multiple | .NET 6.0 | .NET 6.0 |  25,301.04 ns |   676.444 ns |   447.426 ns | 1,024.52x slower |  17.57x |      - |      - |         - |          NA |
| Get_Guid            | .NET 6.0 | .NET 6.0 |     161.06 ns |    15.346 ns |    10.151 ns |     6.52x slower |   0.39x | 0.0029 |      - |     144 B |          NA |
| Get_DateTime        | .NET 6.0 | .NET 6.0 |     208.98 ns |     3.263 ns |     2.158 ns |     8.46x slower |   0.09x | 0.0024 |      - |     128 B |          NA |
| Get_Enum            | .NET 6.0 | .NET 6.0 |     240.22 ns |    29.580 ns |    19.565 ns |     9.73x slower |   0.76x | 0.0026 |      - |     128 B |          NA |
| Get_NullableInt     | .NET 6.0 | .NET 6.0 |     308.00 ns |    32.791 ns |    21.689 ns |    12.47x slower |   0.84x | 0.0029 |      - |     136 B |          NA |
| TryGet_Success      | .NET 6.0 | .NET 6.0 |      38.03 ns |     3.151 ns |     2.084 ns |     1.54x slower |   0.08x |      - |      - |         - |          NA |
| TryGet_Failure      | .NET 6.0 | .NET 6.0 |      27.22 ns |     1.488 ns |     0.984 ns |     1.10x slower |   0.04x |      - |      - |         - |          NA |
| GetRequired_Success | .NET 6.0 | .NET 6.0 |      54.62 ns |     6.729 ns |     4.451 ns |     2.21x slower |   0.17x |      - |      - |         - |          NA |
| Get_MixedTypes      | .NET 6.0 | .NET 6.0 |  59,010.09 ns |   574.664 ns |   380.105 ns | 2,389.51x slower |  16.43x | 1.0376 |      - |   51200 B |          NA |
| Get_LongString      | .NET 6.0 | .NET 6.0 |      27.24 ns |     0.223 ns |     0.148 ns |     1.10x slower |   0.01x |      - |      - |         - |          NA |
| Get_Unicode         | .NET 6.0 | .NET 6.0 |      24.71 ns |     0.263 ns |     0.174 ns |     1.00x slower |   0.01x |      - |      - |         - |          NA |
| Get_SpecialChars    | .NET 6.0 | .NET 6.0 |      29.46 ns |     0.435 ns |     0.288 ns |     1.19x slower |   0.01x |      - |      - |         - |          NA |
| Get_EmptyString     | .NET 6.0 | .NET 6.0 |      28.38 ns |     0.232 ns |     0.154 ns |     1.15x slower |   0.01x |      - |      - |         - |          NA |
|                     |          |          |               |              |              |                  |         |        |        |           |             |
| Get_String          | .NET 8.0 | .NET 8.0 |      18.64 ns |     1.875 ns |     1.240 ns |         baseline |         |      - |      - |         - |          NA |
| Get_String_Generic  | .NET 8.0 | .NET 8.0 |      61.92 ns |     1.049 ns |     0.694 ns |     3.34x slower |   0.21x | 0.0008 | 0.0001 |      40 B |          NA |
| Get_Int             | .NET 8.0 | .NET 8.0 |      73.98 ns |     0.500 ns |     0.331 ns |     3.99x slower |   0.25x | 0.0018 | 0.0001 |      88 B |          NA |
| Get_Long            | .NET 8.0 | .NET 8.0 |      84.70 ns |     0.494 ns |     0.294 ns |     4.56x slower |   0.29x | 0.0018 | 0.0001 |      88 B |          NA |
| Get_Double          | .NET 8.0 | .NET 8.0 |     104.01 ns |     0.819 ns |     0.428 ns |     5.60x slower |   0.35x | 0.0018 | 0.0001 |      88 B |          NA |
| Get_Decimal         | .NET 8.0 | .NET 8.0 |     131.97 ns |     7.346 ns |     4.859 ns |     7.11x slower |   0.51x | 0.0020 |      - |     104 B |          NA |
| Get_Bool            | .NET 8.0 | .NET 8.0 |      93.67 ns |     8.485 ns |     5.612 ns |     5.05x slower |   0.43x | 0.0018 | 0.0001 |      88 B |          NA |
| Get_Int_Multiple    | .NET 8.0 | .NET 8.0 |  77,754.86 ns |   875.684 ns |   579.211 ns | 4,188.98x slower | 264.78x | 1.7090 |      - |   88000 B |          NA |
| Get_Bool_Multiple   | .NET 8.0 | .NET 8.0 |  69,787.95 ns |   508.954 ns |   336.642 ns | 3,759.77x slower | 236.77x | 1.8311 | 0.1221 |   88000 B |          NA |
| Get_Double_Multiple | .NET 8.0 | .NET 8.0 | 113,700.44 ns | 6,276.690 ns | 3,735.157 ns | 6,125.52x slower | 429.79x | 1.8311 | 0.1221 |   88000 B |          NA |
| Get_String_Multiple | .NET 8.0 | .NET 8.0 |  15,913.98 ns |   407.171 ns |   242.301 ns |   857.35x slower |  55.29x |      - |      - |         - |          NA |
| Get_Guid            | .NET 8.0 | .NET 8.0 |     114.90 ns |     6.274 ns |     4.150 ns |     6.19x slower |   0.44x | 0.0020 |      - |     104 B |          NA |
| Get_DateTime        | .NET 8.0 | .NET 8.0 |     202.81 ns |    15.955 ns |    10.553 ns |    10.93x slower |   0.88x | 0.0017 |      - |      88 B |          NA |
| Get_Enum            | .NET 8.0 | .NET 8.0 |     156.74 ns |    14.133 ns |     9.348 ns |     8.44x slower |   0.72x | 0.0017 |      - |      88 B |          NA |
| Get_NullableInt     | .NET 8.0 | .NET 8.0 |     160.26 ns |    17.538 ns |    11.600 ns |     8.63x slower |   0.81x | 0.0019 |      - |      96 B |          NA |
| TryGet_Success      | .NET 8.0 | .NET 8.0 |      20.82 ns |     0.074 ns |     0.039 ns |     1.12x slower |   0.07x |      - |      - |         - |          NA |
| TryGet_Failure      | .NET 8.0 | .NET 8.0 |      17.04 ns |     2.081 ns |     1.089 ns |     1.10x faster |   0.09x |      - |      - |         - |          NA |
| GetRequired_Success | .NET 8.0 | .NET 8.0 |      25.68 ns |     1.722 ns |     1.025 ns |     1.38x slower |   0.10x |      - |      - |         - |          NA |
| Get_MixedTypes      | .NET 8.0 | .NET 8.0 |  38,328.29 ns |   405.653 ns |   268.314 ns | 2,064.91x slower | 130.42x | 0.7324 | 0.0610 |   35200 B |          NA |
| Get_LongString      | .NET 8.0 | .NET 8.0 |      15.85 ns |     0.183 ns |     0.121 ns |     1.18x faster |   0.08x |      - |      - |         - |          NA |
| Get_Unicode         | .NET 8.0 | .NET 8.0 |      16.14 ns |     0.219 ns |     0.130 ns |     1.15x faster |   0.07x |      - |      - |         - |          NA |
| Get_SpecialChars    | .NET 8.0 | .NET 8.0 |      17.10 ns |     0.292 ns |     0.153 ns |     1.09x faster |   0.07x |      - |      - |         - |          NA |
| Get_EmptyString     | .NET 8.0 | .NET 8.0 |      17.25 ns |     0.209 ns |     0.110 ns |     1.08x faster |   0.07x |      - |      - |         - |          NA |
|                     |          |          |               |              |              |                  |         |        |        |           |             |
| Get_String          | .NET 9.0 | .NET 9.0 |      18.35 ns |     1.214 ns |     0.722 ns |         baseline |         |      - |      - |         - |          NA |
| Get_String_Generic  | .NET 9.0 | .NET 9.0 |      67.53 ns |     5.248 ns |     3.471 ns |     3.69x slower |   0.23x | 0.0008 |      - |      40 B |          NA |
| Get_Int             | .NET 9.0 | .NET 9.0 |      87.92 ns |     5.475 ns |     3.621 ns |     4.80x slower |   0.26x | 0.0013 |      - |      64 B |          NA |
| Get_Long            | .NET 9.0 | .NET 9.0 |      96.91 ns |     6.710 ns |     4.439 ns |     5.29x slower |   0.30x | 0.0013 |      - |      64 B |          NA |
| Get_Double          | .NET 9.0 | .NET 9.0 |     121.39 ns |     9.041 ns |     5.980 ns |     6.62x slower |   0.40x | 0.0013 |      - |      64 B |          NA |
| Get_Decimal         | .NET 9.0 | .NET 9.0 |     127.36 ns |    12.800 ns |     8.466 ns |     6.95x slower |   0.51x | 0.0014 |      - |      72 B |          NA |
| Get_Bool            | .NET 9.0 | .NET 9.0 |      65.84 ns |     1.621 ns |     1.072 ns |     3.59x slower |   0.15x | 0.0013 |      - |      64 B |          NA |
| Get_Int_Multiple    | .NET 9.0 | .NET 9.0 |  71,360.67 ns | 1,091.170 ns |   721.741 ns | 3,894.31x slower | 150.89x | 1.3428 |      - |   64000 B |          NA |
| Get_Bool_Multiple   | .NET 9.0 | .NET 9.0 |  65,007.11 ns |   791.102 ns |   523.265 ns | 3,547.58x slower | 135.88x | 1.3428 |      - |   64000 B |          NA |
| Get_Double_Multiple | .NET 9.0 | .NET 9.0 |  96,881.04 ns | 1,662.432 ns | 1,099.596 ns | 5,287.01x slower | 206.49x | 1.3428 |      - |   64000 B |          NA |
| Get_String_Multiple | .NET 9.0 | .NET 9.0 |  15,387.28 ns |   240.388 ns |   143.051 ns |   839.72x slower |  32.39x |      - |      - |         - |          NA |
| Get_Guid            | .NET 9.0 | .NET 9.0 |      80.23 ns |     1.389 ns |     0.919 ns |     4.38x slower |   0.17x | 0.0014 |      - |      72 B |          NA |
| Get_DateTime        | .NET 9.0 | .NET 9.0 |     134.82 ns |     2.896 ns |     1.723 ns |     7.36x slower |   0.29x | 0.0012 |      - |      64 B |          NA |
| Get_Enum            | .NET 9.0 | .NET 9.0 |      92.73 ns |     1.257 ns |     0.832 ns |     5.06x slower |   0.19x | 0.0013 |      - |      64 B |          NA |
| Get_NullableInt     | .NET 9.0 | .NET 9.0 |     128.76 ns |    21.984 ns |    14.541 ns |     7.03x slower |   0.80x | 0.0019 |      - |      96 B |          NA |
| TryGet_Success      | .NET 9.0 | .NET 9.0 |      26.68 ns |     4.016 ns |     2.390 ns |     1.46x slower |   0.14x |      - |      - |         - |          NA |
| TryGet_Failure      | .NET 9.0 | .NET 9.0 |      13.98 ns |     0.054 ns |     0.036 ns |     1.31x faster |   0.05x |      - |      - |         - |          NA |
| GetRequired_Success | .NET 9.0 | .NET 9.0 |      20.63 ns |     0.219 ns |     0.145 ns |     1.13x slower |   0.04x |      - |      - |         - |          NA |
| Get_MixedTypes      | .NET 9.0 | .NET 9.0 |  36,019.47 ns |   558.427 ns |   369.365 ns | 1,965.66x slower |  76.23x | 0.4883 |      - |   25600 B |          NA |
| Get_LongString      | .NET 9.0 | .NET 9.0 |      20.04 ns |     5.511 ns |     3.645 ns |     1.09x slower |   0.19x |      - |      - |         - |          NA |
| Get_Unicode         | .NET 9.0 | .NET 9.0 |      15.44 ns |     0.205 ns |     0.122 ns |     1.19x faster |   0.05x |      - |      - |         - |          NA |
| Get_SpecialChars    | .NET 9.0 | .NET 9.0 |      16.10 ns |     0.344 ns |     0.228 ns |     1.14x faster |   0.05x |      - |      - |         - |          NA |
| Get_EmptyString     | .NET 9.0 | .NET 9.0 |      17.16 ns |     0.320 ns |     0.212 ns |     1.07x faster |   0.04x |      - |      - |         - |          NA |
