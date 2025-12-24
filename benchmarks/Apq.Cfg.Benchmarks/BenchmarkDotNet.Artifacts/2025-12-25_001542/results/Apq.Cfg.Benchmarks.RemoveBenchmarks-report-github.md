```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]   : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 6.0 : .NET 6.0.36 (6.0.3624.51421), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  .NET 9.0 : .NET 9.0.11 (9.0.1125.51716), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

InvocationCount=1  IterationCount=10  LaunchCount=1  
UnrollFactor=1  WarmupCount=5  

```
| Method                 | Job      | Runtime  | KeyCount | Mean         | Error         | StdDev        | Median       | Ratio            | RatioSD   | Allocated | Alloc Ratio  |
|----------------------- |--------- |--------- |--------- |-------------:|--------------:|--------------:|-------------:|-----------------:|----------:|----------:|-------------:|
| **Json_RemoveAndSave**     | **.NET 6.0** | **.NET 6.0** | **10**       | **1,152.640 μs** |   **335.3305 μs** |   **221.8003 μs** | **1,192.950 μs** |   **686.12x slower** |   **166.54x** |   **74648 B** |  **61.39x more** |
| Ini_RemoveAndSave      | .NET 6.0 | .NET 6.0 | 10       |   885.178 μs |   297.5253 μs |   177.0526 μs |   839.600 μs |   526.91x slower |   130.47x |   41008 B |  33.72x more |
| Xml_RemoveAndSave      | .NET 6.0 | .NET 6.0 | 10       |   734.740 μs |   135.8136 μs |    89.8323 μs |   747.950 μs |   437.36x slower |    85.64x |   65824 B |  54.13x more |
| Yaml_RemoveAndSave     | .NET 6.0 | .NET 6.0 | 10       |   776.740 μs |   131.2920 μs |    86.8415 μs |   768.100 μs |   462.36x slower |    87.79x |   60920 B |  50.10x more |
| Toml_RemoveAndSave     | .NET 6.0 | .NET 6.0 | 10       |   962.389 μs |   181.0268 μs |   107.7261 μs |   995.600 μs |   572.87x slower |   108.67x |   83080 B |  68.32x more |
| Json_RemoveBatch       | .NET 6.0 | .NET 6.0 | 10       |     3.475 μs |     0.4306 μs |     0.2252 μs |     3.350 μs |     2.07x slower |     0.35x |    2504 B |   2.06x more |
| Ini_RemoveBatch        | .NET 6.0 | .NET 6.0 | 10       |     4.610 μs |     0.6056 μs |     0.4006 μs |     4.400 μs |     2.74x slower |     0.49x |    5256 B |   4.32x more |
| Xml_RemoveBatch        | .NET 6.0 | .NET 6.0 | 10       |     3.278 μs |     0.7827 μs |     0.4658 μs |     3.200 μs |     1.95x slower |     0.40x |    2584 B |   2.12x more |
| Yaml_RemoveBatch       | .NET 6.0 | .NET 6.0 | 10       |     3.650 μs |     0.7415 μs |     0.4905 μs |     3.500 μs |     2.17x slower |     0.44x |    2584 B |   2.12x more |
| Toml_RemoveBatch       | .NET 6.0 | .NET 6.0 | 10       |     4.189 μs |     0.1314 μs |     0.0782 μs |     4.200 μs |     2.49x slower |     0.39x |    5256 B |   4.32x more |
| Json_RemoveNonExistent | .NET 6.0 | .NET 6.0 | 10       |     4.722 μs |     0.4810 μs |     0.2863 μs |     4.700 μs |     2.81x slower |     0.47x |    5368 B |   4.41x more |
| Ini_RemoveNonExistent  | .NET 6.0 | .NET 6.0 | 10       |     4.110 μs |     0.4243 μs |     0.2807 μs |     4.150 μs |     2.45x slower |     0.41x |    2744 B |   2.26x more |
| Json_RemoveSingle      | .NET 6.0 | .NET 6.0 | 10       |     1.722 μs |     0.4883 μs |     0.2906 μs |     1.700 μs |         baseline |           |    1216 B |              |
| Ini_RemoveSingle       | .NET 6.0 | .NET 6.0 | 10       |     2.510 μs |     0.6066 μs |     0.4012 μs |     2.450 μs |     1.49x slower |     0.33x |    1216 B |   1.00x more |
| Xml_RemoveSingle       | .NET 6.0 | .NET 6.0 | 10       |     1.830 μs |     0.5523 μs |     0.3653 μs |     1.800 μs |     1.09x slower |     0.27x |    1216 B |   1.00x more |
| Yaml_RemoveSingle      | .NET 6.0 | .NET 6.0 | 10       |     1.363 μs |     0.1423 μs |     0.0744 μs |     1.400 μs |     1.27x faster |     0.22x |    1216 B |   1.00x more |
| Toml_RemoveSingle      | .NET 6.0 | .NET 6.0 | 10       |     1.790 μs |     0.3061 μs |     0.2025 μs |     1.750 μs |     1.07x slower |     0.20x |    1216 B |   1.00x more |
|                        |          |          |          |              |               |               |              |                  |           |           |              |
| Json_RemoveAndSave     | .NET 8.0 | .NET 8.0 | 10       |   755.533 μs |    94.1117 μs |    56.0044 μs |   739.500 μs |   484.01x slower |    37.38x |   64752 B | 130.55x more |
| Ini_RemoveAndSave      | .NET 8.0 | .NET 8.0 | 10       |   662.080 μs |    90.1261 μs |    59.6128 μs |   673.050 μs |   424.14x slower |    38.87x |   37496 B |  75.60x more |
| Xml_RemoveAndSave      | .NET 8.0 | .NET 8.0 | 10       |   700.720 μs |    81.8806 μs |    54.1590 μs |   695.100 μs |   448.90x slower |    36.06x |   52712 B | 106.27x more |
| Yaml_RemoveAndSave     | .NET 8.0 | .NET 8.0 | 10       |   728.788 μs |    69.2286 μs |    36.2079 μs |   740.200 μs |   466.88x slower |    26.42x |   68480 B | 138.06x more |
| Toml_RemoveAndSave     | .NET 8.0 | .NET 8.0 | 10       |   874.240 μs |   214.2972 μs |   141.7443 μs |   831.750 μs |   560.06x slower |    88.53x |   74040 B | 149.27x more |
| Json_RemoveBatch       | .NET 8.0 | .NET 8.0 | 10       |     5.933 μs |     2.1206 μs |     1.2619 μs |     5.400 μs |     3.80x slower |     0.78x |    2360 B |   4.76x more |
| Ini_RemoveBatch        | .NET 8.0 | .NET 8.0 | 10       |     5.010 μs |     0.9732 μs |     0.6437 μs |     4.900 μs |     3.21x slower |     0.41x |    2440 B |   4.92x more |
| Xml_RemoveBatch        | .NET 8.0 | .NET 8.0 | 10       |     4.370 μs |     0.6007 μs |     0.3974 μs |     4.450 μs |     2.80x slower |     0.26x |    2440 B |   4.92x more |
| Yaml_RemoveBatch       | .NET 8.0 | .NET 8.0 | 10       |     4.390 μs |     0.4243 μs |     0.2807 μs |     4.500 μs |     2.81x slower |     0.19x |    2440 B |   4.92x more |
| Toml_RemoveBatch       | .NET 8.0 | .NET 8.0 | 10       |     4.233 μs |     0.5247 μs |     0.3122 μs |     4.200 μs |     2.71x slower |     0.21x |    2440 B |   4.92x more |
| Json_RemoveNonExistent | .NET 8.0 | .NET 8.0 | 10       |     3.730 μs |     0.2022 μs |     0.1337 μs |     3.750 μs |     2.39x slower |     0.11x |    2600 B |   5.24x more |
| Ini_RemoveNonExistent  | .NET 8.0 | .NET 8.0 | 10       |     4.078 μs |     0.8843 μs |     0.5263 μs |     3.900 μs |     2.61x slower |     0.33x |    2600 B |   5.24x more |
| Json_RemoveSingle      | .NET 8.0 | .NET 8.0 | 10       |     1.562 μs |     0.0990 μs |     0.0518 μs |     1.600 μs |         baseline |           |     496 B |              |
| Ini_RemoveSingle       | .NET 8.0 | .NET 8.0 | 10       |     1.550 μs |     0.2047 μs |     0.1354 μs |     1.500 μs |     1.01x faster |     0.09x |    1072 B |   2.16x more |
| Xml_RemoveSingle       | .NET 8.0 | .NET 8.0 | 10       |     1.560 μs |     0.2956 μs |     0.1955 μs |     1.450 μs |     1.02x faster |     0.12x |    1072 B |   2.16x more |
| Yaml_RemoveSingle      | .NET 8.0 | .NET 8.0 | 10       |     1.462 μs |     0.0990 μs |     0.0518 μs |     1.500 μs |     1.07x faster |     0.05x |    1072 B |   2.16x more |
| Toml_RemoveSingle      | .NET 8.0 | .NET 8.0 | 10       |     1.644 μs |     0.3042 μs |     0.1810 μs |     1.500 μs |     1.05x slower |     0.12x |    1072 B |   2.16x more |
|                        |          |          |          |              |               |               |              |                  |           |           |              |
| Json_RemoveAndSave     | .NET 9.0 | .NET 9.0 | 10       |   744.800 μs |   113.6646 μs |    67.6400 μs |   751.900 μs |   565.22x slower |    65.05x |   63440 B |  45.06x more |
| Ini_RemoveAndSave      | .NET 9.0 | .NET 9.0 | 10       |   712.370 μs |   145.6497 μs |    96.3383 μs |   689.750 μs |   540.61x slower |    81.13x |   28080 B |  19.94x more |
| Xml_RemoveAndSave      | .NET 9.0 | .NET 9.0 | 10       |   729.790 μs |   132.2309 μs |    87.4626 μs |   694.450 μs |   553.83x slower |    76.19x |   64624 B |  45.90x more |
| Yaml_RemoveAndSave     | .NET 9.0 | .NET 9.0 | 10       |   733.033 μs |    59.8394 μs |    35.6094 μs |   741.600 μs |   556.29x slower |    49.47x |   67640 B |  48.04x more |
| Toml_RemoveAndSave     | .NET 9.0 | .NET 9.0 | 10       | 1,022.800 μs |   374.8687 μs |   196.0636 μs |   982.200 μs |   776.19x slower |   152.55x |   65184 B |  46.30x more |
| Json_RemoveBatch       | .NET 9.0 | .NET 9.0 | 10       |     3.878 μs |     0.5026 μs |     0.2991 μs |     3.900 μs |     2.94x slower |     0.31x |    2696 B |   1.91x more |
| Ini_RemoveBatch        | .NET 9.0 | .NET 9.0 | 10       |     3.725 μs |     0.1695 μs |     0.0886 μs |     3.700 μs |     2.83x slower |     0.22x |    2776 B |   1.97x more |
| Xml_RemoveBatch        | .NET 9.0 | .NET 9.0 | 10       |     4.080 μs |     0.6286 μs |     0.4158 μs |     4.100 μs |     3.10x slower |     0.38x |    2776 B |   1.97x more |
| Yaml_RemoveBatch       | .NET 9.0 | .NET 9.0 | 10       |     3.789 μs |     0.3193 μs |     0.1900 μs |     3.700 μs |     2.88x slower |     0.26x |    2440 B |   1.73x more |
| Toml_RemoveBatch       | .NET 9.0 | .NET 9.0 | 10       |     4.056 μs |     0.6181 μs |     0.3678 μs |     4.000 μs |     3.08x slower |     0.35x |    2776 B |   1.97x more |
| Json_RemoveNonExistent | .NET 9.0 | .NET 9.0 | 10       |     3.500 μs |     0.4563 μs |     0.3018 μs |     3.500 μs |     2.66x slower |     0.30x |    2936 B |   2.09x more |
| Ini_RemoveNonExistent  | .NET 9.0 | .NET 9.0 | 10       |     3.611 μs |     0.5538 μs |     0.3296 μs |     3.600 μs |     2.74x slower |     0.32x |    2648 B |   1.88x more |
| Json_RemoveSingle      | .NET 9.0 | .NET 9.0 | 10       |     1.325 μs |     0.1979 μs |     0.1035 μs |     1.350 μs |         baseline |           |    1408 B |              |
| Ini_RemoveSingle       | .NET 9.0 | .NET 9.0 | 10       |     1.356 μs |     0.1221 μs |     0.0726 μs |     1.300 μs |     1.03x slower |     0.09x |    1408 B |   1.00x more |
| Xml_RemoveSingle       | .NET 9.0 | .NET 9.0 | 10       |     1.578 μs |     0.3830 μs |     0.2279 μs |     1.500 μs |     1.20x slower |     0.19x |    1408 B |   1.00x more |
| Yaml_RemoveSingle      | .NET 9.0 | .NET 9.0 | 10       |     1.278 μs |     0.1120 μs |     0.0667 μs |     1.300 μs |     1.04x faster |     0.09x |    1120 B |   1.26x less |
| Toml_RemoveSingle      | .NET 9.0 | .NET 9.0 | 10       |     1.440 μs |     0.2489 μs |     0.1647 μs |     1.400 μs |     1.09x slower |     0.15x |    1408 B |   1.00x more |
|                        |          |          |          |              |               |               |              |                  |           |           |              |
| **Json_RemoveAndSave**     | **.NET 6.0** | **.NET 6.0** | **50**       |   **957.511 μs** |   **183.0922 μs** |   **108.9552 μs** |   **980.700 μs** |   **591.21x slower** |    **96.62x** |  **107376 B** |  **88.30x more** |
| Ini_RemoveAndSave      | .NET 6.0 | .NET 6.0 | 50       | 4,997.590 μs | 9,806.9265 μs | 6,486.6747 μs | 1,093.950 μs | 3,085.74x slower | 3,867.25x |   69144 B |  56.86x more |
| Xml_RemoveAndSave      | .NET 6.0 | .NET 6.0 | 50       |   737.025 μs |   139.2105 μs |    72.8098 μs |   739.600 μs |   455.07x slower |    70.05x |  114640 B |  94.28x more |
| Yaml_RemoveAndSave     | .NET 6.0 | .NET 6.0 | 50       |   876.100 μs |   144.0748 μs |    95.2966 μs |   849.050 μs |   540.94x slower |    86.88x |  144000 B | 118.42x more |
| Toml_RemoveAndSave     | .NET 6.0 | .NET 6.0 | 50       | 1,050.040 μs |   239.1827 μs |   158.2045 μs | 1,015.550 μs |   648.34x slower |   122.72x |  187760 B | 154.41x more |
| Json_RemoveBatch       | .NET 6.0 | .NET 6.0 | 50       |     8.180 μs |     0.8120 μs |     0.5371 μs |     8.200 μs |     5.05x slower |     0.69x |    8328 B |   6.85x more |
| Ini_RemoveBatch        | .NET 6.0 | .NET 6.0 | 50       |     9.520 μs |     0.6678 μs |     0.4417 μs |     9.500 μs |     5.88x slower |     0.76x |   14632 B |  12.03x more |
| Xml_RemoveBatch        | .NET 6.0 | .NET 6.0 | 50       |    10.356 μs |     1.3367 μs |     0.7955 μs |    10.200 μs |     6.39x slower |     0.91x |   14584 B |  11.99x more |
| Yaml_RemoveBatch       | .NET 6.0 | .NET 6.0 | 50       |    11.780 μs |     0.9610 μs |     0.6356 μs |    11.650 μs |     7.27x slower |     0.96x |   25080 B |  20.62x more |
| Toml_RemoveBatch       | .NET 6.0 | .NET 6.0 | 50       |    11.930 μs |     0.4991 μs |     0.3302 μs |    11.900 μs |     7.37x slower |     0.92x |   25752 B |  21.18x more |
| Json_RemoveNonExistent | .NET 6.0 | .NET 6.0 | 50       |    13.360 μs |     1.0084 μs |     0.6670 μs |    13.300 μs |     8.25x slower |     1.08x |   25608 B |  21.06x more |
| Ini_RemoveNonExistent  | .NET 6.0 | .NET 6.0 | 50       |     9.490 μs |     0.8469 μs |     0.5602 μs |     9.350 μs |     5.86x slower |     0.79x |   15640 B |  12.86x more |
| Json_RemoveSingle      | .NET 6.0 | .NET 6.0 | 50       |     1.644 μs |     0.3673 μs |     0.2186 μs |     1.600 μs |         baseline |           |    1216 B |              |
| Ini_RemoveSingle       | .NET 6.0 | .NET 6.0 | 50       |     1.572 μs |     0.3337 μs |     0.1986 μs |     1.550 μs |     1.06x faster |     0.18x |    1216 B |   1.00x more |
| Xml_RemoveSingle       | .NET 6.0 | .NET 6.0 | 50       |     1.794 μs |     0.5828 μs |     0.3468 μs |     1.750 μs |     1.11x slower |     0.25x |    1216 B |   1.00x more |
| Yaml_RemoveSingle      | .NET 6.0 | .NET 6.0 | 50       |     1.500 μs |     0.1879 μs |     0.1118 μs |     1.500 μs |     1.10x faster |     0.16x |    1216 B |   1.00x more |
| Toml_RemoveSingle      | .NET 6.0 | .NET 6.0 | 50       |     1.570 μs |     0.3422 μs |     0.2263 μs |     1.500 μs |     1.07x faster |     0.19x |    1216 B |   1.00x more |
|                        |          |          |          |              |               |               |              |                  |           |           |              |
| Json_RemoveAndSave     | .NET 8.0 | .NET 8.0 | 50       |   853.256 μs |   178.7644 μs |   106.3798 μs |   852.900 μs |   548.91x slower |    72.70x |   88048 B |  82.13x more |
| Ini_RemoveAndSave      | .NET 8.0 | .NET 8.0 | 50       |   742.833 μs |   129.2775 μs |    76.9310 μs |   717.500 μs |   477.88x slower |    54.89x |   47208 B |  44.04x more |
| Xml_RemoveAndSave      | .NET 8.0 | .NET 8.0 | 50       |   760.788 μs |   119.2857 μs |    62.3887 μs |   762.050 μs |   489.43x slower |    47.71x |  104000 B |  97.01x more |
| Yaml_RemoveAndSave     | .NET 8.0 | .NET 8.0 | 50       |   997.030 μs |   108.9960 μs |    72.0941 μs |   986.150 μs |   641.41x slower |    58.38x |  143728 B | 134.07x more |
| Toml_RemoveAndSave     | .NET 8.0 | .NET 8.0 | 50       |   961.544 μs |    84.2448 μs |    50.1328 μs |   969.600 μs |   618.58x slower |    47.80x |  167472 B | 156.22x more |
| Json_RemoveBatch       | .NET 8.0 | .NET 8.0 | 50       |    14.170 μs |     1.1157 μs |     0.7379 μs |    14.150 μs |     9.12x slower |     0.71x |   27712 B |  25.85x more |
| Ini_RemoveBatch        | .NET 8.0 | .NET 8.0 | 50       |    12.067 μs |     1.0359 μs |     0.6164 μs |    12.000 μs |     7.76x slower |     0.59x |   17272 B |  16.11x more |
| Xml_RemoveBatch        | .NET 8.0 | .NET 8.0 | 50       |    10.844 μs |     0.3266 μs |     0.1944 μs |    10.800 μs |     6.98x slower |     0.43x |   17272 B |  16.11x more |
| Yaml_RemoveBatch       | .NET 8.0 | .NET 8.0 | 50       |    11.200 μs |     0.3254 μs |     0.1936 μs |    11.200 μs |     7.21x slower |     0.44x |   17272 B |  16.11x more |
| Toml_RemoveBatch       | .NET 8.0 | .NET 8.0 | 50       |    11.878 μs |     1.5601 μs |     0.9284 μs |    12.100 μs |     7.64x slower |     0.73x |   17272 B |  16.11x more |
| Json_RemoveNonExistent | .NET 8.0 | .NET 8.0 | 50       |    10.933 μs |     0.6877 μs |     0.4093 μs |    10.900 μs |     7.03x slower |     0.49x |   15976 B |  14.90x more |
| Ini_RemoveNonExistent  | .NET 8.0 | .NET 8.0 | 50       |     9.906 μs |     0.6007 μs |     0.3575 μs |     9.850 μs |     6.37x slower |     0.44x |   15976 B |  14.90x more |
| Json_RemoveSingle      | .NET 8.0 | .NET 8.0 | 50       |     1.560 μs |     0.1503 μs |     0.0994 μs |     1.550 μs |         baseline |           |    1072 B |              |
| Ini_RemoveSingle       | .NET 8.0 | .NET 8.0 | 50       |     1.500 μs |     0.1445 μs |     0.0756 μs |     1.500 μs |     1.04x faster |     0.08x |     496 B |   2.16x less |
| Xml_RemoveSingle       | .NET 8.0 | .NET 8.0 | 50       |     1.980 μs |     0.2833 μs |     0.1874 μs |     1.900 μs |     1.27x slower |     0.14x |    1072 B |   1.00x more |
| Yaml_RemoveSingle      | .NET 8.0 | .NET 8.0 | 50       |     3.140 μs |     1.4132 μs |     0.9348 μs |     3.150 μs |     2.02x slower |     0.59x |     736 B |   1.46x less |
| Toml_RemoveSingle      | .NET 8.0 | .NET 8.0 | 50       |     1.640 μs |     0.2869 μs |     0.1897 μs |     1.600 μs |     1.06x slower |     0.13x |    1072 B |   1.00x more |
|                        |          |          |          |              |               |               |              |                  |           |           |              |
| Json_RemoveAndSave     | .NET 9.0 | .NET 9.0 | 50       |   778.933 μs |   128.3841 μs |    76.3993 μs |   772.500 μs |   577.78x slower |    57.99x |   73216 B |  52.00x more |
| Ini_RemoveAndSave      | .NET 9.0 | .NET 9.0 | 50       |   752.430 μs |   111.8271 μs |    73.9667 μs |   750.250 μs |   558.12x slower |    56.39x |   47496 B |  33.73x more |
| Xml_RemoveAndSave      | .NET 9.0 | .NET 9.0 | 50       |   720.812 μs |    85.5552 μs |    44.7470 μs |   704.500 μs |   534.67x slower |    37.13x |  112120 B |  79.63x more |
| Yaml_RemoveAndSave     | .NET 9.0 | .NET 9.0 | 50       |   898.978 μs |   133.6799 μs |    79.5508 μs |   876.000 μs |   666.82x slower |    61.33x |  142864 B | 101.47x more |
| Toml_RemoveAndSave     | .NET 9.0 | .NET 9.0 | 50       |   901.312 μs |   121.7539 μs |    63.6797 μs |   878.750 μs |   668.56x slower |    51.08x |  159224 B | 113.09x more |
| Json_RemoveBatch       | .NET 9.0 | .NET 9.0 | 50       |    12.556 μs |     0.5706 μs |     0.3395 μs |    12.700 μs |     9.31x slower |     0.42x |   28048 B |  19.92x more |
| Ini_RemoveBatch        | .NET 9.0 | .NET 9.0 | 50       |    11.056 μs |     0.8532 μs |     0.5077 μs |    11.200 μs |     8.20x slower |     0.47x |   17320 B |  12.30x more |
| Xml_RemoveBatch        | .NET 9.0 | .NET 9.0 | 50       |    10.800 μs |     0.7325 μs |     0.4359 μs |    10.800 μs |     8.01x slower |     0.43x |   17608 B |  12.51x more |
| Yaml_RemoveBatch       | .NET 9.0 | .NET 9.0 | 50       |    10.767 μs |     0.6877 μs |     0.4093 μs |    10.800 μs |     7.99x slower |     0.41x |   17608 B |  12.51x more |
| Toml_RemoveBatch       | .NET 9.0 | .NET 9.0 | 50       |    10.444 μs |     0.3476 μs |     0.2068 μs |    10.400 μs |     7.75x slower |     0.32x |   17608 B |  12.51x more |
| Json_RemoveNonExistent | .NET 9.0 | .NET 9.0 | 50       |     9.562 μs |     0.4206 μs |     0.2200 μs |     9.600 μs |     7.09x slower |     0.31x |   16024 B |  11.38x more |
| Ini_RemoveNonExistent  | .NET 9.0 | .NET 9.0 | 50       |     9.789 μs |     0.5726 μs |     0.3408 μs |     9.800 μs |     7.26x slower |     0.36x |   16024 B |  11.38x more |
| Json_RemoveSingle      | .NET 9.0 | .NET 9.0 | 50       |     1.350 μs |     0.1022 μs |     0.0535 μs |     1.350 μs |         baseline |           |    1408 B |              |
| Ini_RemoveSingle       | .NET 9.0 | .NET 9.0 | 50       |     1.356 μs |     0.1221 μs |     0.0726 μs |     1.300 μs |     1.01x slower |     0.06x |    1408 B |   1.00x more |
| Xml_RemoveSingle       | .NET 9.0 | .NET 9.0 | 50       |     1.550 μs |     0.4047 μs |     0.2677 μs |     1.550 μs |     1.15x slower |     0.19x |    1408 B |   1.00x more |
| Yaml_RemoveSingle      | .NET 9.0 | .NET 9.0 | 50       |     1.344 μs |     0.1482 μs |     0.0882 μs |     1.300 μs |     1.01x faster |     0.07x |    1408 B |   1.00x more |
| Toml_RemoveSingle      | .NET 9.0 | .NET 9.0 | 50       |     1.610 μs |     0.3301 μs |     0.2183 μs |     1.550 μs |     1.19x slower |     0.16x |    1408 B |   1.00x more |
