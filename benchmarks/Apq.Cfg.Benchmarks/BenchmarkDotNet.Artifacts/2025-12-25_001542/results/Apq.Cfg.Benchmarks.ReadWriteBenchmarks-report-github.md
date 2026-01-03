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
| Method      | Job      | Runtime  | Mean      | Error     | StdDev    | Ratio        | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------ |--------- |--------- |----------:|----------:|----------:|-------------:|--------:|-------:|----------:|------------:|
| Json_Exists | .NET 6.0 | .NET 6.0 |  25.90 ns |  0.399 ns |  0.238 ns | 1.01x faster |   0.02x |      - |         - |          NA |
| Ini_Exists  | .NET 6.0 | .NET 6.0 |  25.64 ns |  0.153 ns |  0.101 ns | 1.02x faster |   0.01x |      - |         - |          NA |
| Xml_Exists  | .NET 6.0 | .NET 6.0 |  27.16 ns |  1.345 ns |  0.890 ns | 1.04x slower |   0.04x |      - |         - |          NA |
| Yaml_Exists | .NET 6.0 | .NET 6.0 |  26.27 ns |  1.040 ns |  0.688 ns | 1.00x slower |   0.03x |      - |         - |          NA |
| Toml_Exists | .NET 6.0 | .NET 6.0 |  25.82 ns |  0.229 ns |  0.151 ns | 1.02x faster |   0.01x |      - |         - |          NA |
| Json_Get    | .NET 6.0 | .NET 6.0 |  26.21 ns |  0.544 ns |  0.360 ns |     baseline |         |      - |         - |          NA |
| Ini_Get     | .NET 6.0 | .NET 6.0 |  26.20 ns |  1.452 ns |  0.960 ns | 1.00x faster |   0.04x |      - |         - |          NA |
| Xml_Get     | .NET 6.0 | .NET 6.0 |  25.46 ns |  0.360 ns |  0.238 ns | 1.03x faster |   0.02x |      - |         - |          NA |
| Yaml_Get    | .NET 6.0 | .NET 6.0 |  25.85 ns |  0.360 ns |  0.215 ns | 1.01x faster |   0.02x |      - |         - |          NA |
| Toml_Get    | .NET 6.0 | .NET 6.0 |  25.53 ns |  0.508 ns |  0.302 ns | 1.03x faster |   0.02x |      - |         - |          NA |
| Json_GetInt | .NET 6.0 | .NET 6.0 | 163.80 ns | 19.651 ns | 12.998 ns | 6.25x slower |   0.48x | 0.0025 |     128 B |          NA |
| Ini_GetInt  | .NET 6.0 | .NET 6.0 | 163.76 ns | 25.410 ns | 16.807 ns | 6.25x slower |   0.62x | 0.0026 |     128 B |          NA |
| Xml_GetInt  | .NET 6.0 | .NET 6.0 | 160.19 ns | 27.780 ns | 18.375 ns | 6.11x slower |   0.67x | 0.0026 |     128 B |          NA |
| Yaml_GetInt | .NET 6.0 | .NET 6.0 | 135.38 ns | 28.793 ns | 19.045 ns | 5.17x slower |   0.70x | 0.0026 |     128 B |          NA |
| Toml_GetInt | .NET 6.0 | .NET 6.0 | 117.66 ns |  1.033 ns |  0.683 ns | 4.49x slower |   0.06x | 0.0025 |     128 B |          NA |
| Json_Set    | .NET 6.0 | .NET 6.0 |  26.10 ns |  0.230 ns |  0.152 ns | 1.00x faster |   0.01x |      - |         - |          NA |
| Ini_Set     | .NET 6.0 | .NET 6.0 |  26.12 ns |  0.255 ns |  0.169 ns | 1.00x faster |   0.01x |      - |         - |          NA |
| Xml_Set     | .NET 6.0 | .NET 6.0 |  25.82 ns |  0.475 ns |  0.314 ns | 1.02x faster |   0.02x |      - |         - |          NA |
| Yaml_Set    | .NET 6.0 | .NET 6.0 |  26.05 ns |  0.286 ns |  0.189 ns | 1.01x faster |   0.01x |      - |         - |          NA |
| Toml_Set    | .NET 6.0 | .NET 6.0 |  26.04 ns |  0.316 ns |  0.188 ns | 1.01x faster |   0.01x |      - |         - |          NA |
|             |          |          |           |           |           |              |         |        |           |             |
| Json_Exists | .NET 8.0 | .NET 8.0 |  16.40 ns |  0.243 ns |  0.161 ns | 1.02x slower |   0.01x |      - |         - |          NA |
| Ini_Exists  | .NET 8.0 | .NET 8.0 |  16.32 ns |  0.305 ns |  0.182 ns | 1.01x slower |   0.01x |      - |         - |          NA |
| Xml_Exists  | .NET 8.0 | .NET 8.0 |  16.79 ns |  0.863 ns |  0.571 ns | 1.04x slower |   0.03x |      - |         - |          NA |
| Yaml_Exists | .NET 8.0 | .NET 8.0 |  16.78 ns |  0.374 ns |  0.247 ns | 1.04x slower |   0.02x |      - |         - |          NA |
| Toml_Exists | .NET 8.0 | .NET 8.0 |  16.82 ns |  0.226 ns |  0.118 ns | 1.04x slower |   0.01x |      - |         - |          NA |
| Json_Get    | .NET 8.0 | .NET 8.0 |  16.14 ns |  0.178 ns |  0.118 ns |     baseline |         |      - |         - |          NA |
| Ini_Get     | .NET 8.0 | .NET 8.0 |  16.19 ns |  0.368 ns |  0.243 ns | 1.00x slower |   0.02x |      - |         - |          NA |
| Xml_Get     | .NET 8.0 | .NET 8.0 |  16.07 ns |  0.164 ns |  0.086 ns | 1.00x faster |   0.01x |      - |         - |          NA |
| Yaml_Get    | .NET 8.0 | .NET 8.0 |  16.20 ns |  0.343 ns |  0.204 ns | 1.00x slower |   0.01x |      - |         - |          NA |
| Toml_Get    | .NET 8.0 | .NET 8.0 |  19.95 ns |  3.189 ns |  1.668 ns | 1.24x slower |   0.10x |      - |         - |          NA |
| Json_GetInt | .NET 8.0 | .NET 8.0 |  75.33 ns |  0.832 ns |  0.550 ns | 4.67x slower |   0.05x | 0.0018 |      88 B |          NA |
| Ini_GetInt  | .NET 8.0 | .NET 8.0 |  74.84 ns |  0.637 ns |  0.379 ns | 4.64x slower |   0.04x | 0.0018 |      88 B |          NA |
| Xml_GetInt  | .NET 8.0 | .NET 8.0 | 102.50 ns | 31.981 ns | 21.153 ns | 6.35x slower |   1.25x | 0.0018 |      88 B |          NA |
| Yaml_GetInt | .NET 8.0 | .NET 8.0 |  75.77 ns |  1.251 ns |  0.828 ns | 4.70x slower |   0.06x | 0.0018 |      88 B |          NA |
| Toml_GetInt | .NET 8.0 | .NET 8.0 |  76.32 ns |  1.411 ns |  0.933 ns | 4.73x slower |   0.06x | 0.0018 |      88 B |          NA |
| Json_Set    | .NET 8.0 | .NET 8.0 |  19.40 ns |  0.189 ns |  0.125 ns | 1.20x slower |   0.01x |      - |         - |          NA |
| Ini_Set     | .NET 8.0 | .NET 8.0 |  19.45 ns |  0.281 ns |  0.186 ns | 1.21x slower |   0.01x |      - |         - |          NA |
| Xml_Set     | .NET 8.0 | .NET 8.0 |  21.48 ns |  1.176 ns |  0.778 ns | 1.33x slower |   0.05x |      - |         - |          NA |
| Yaml_Set    | .NET 8.0 | .NET 8.0 |  21.23 ns |  0.746 ns |  0.444 ns | 1.32x slower |   0.03x |      - |         - |          NA |
| Toml_Set    | .NET 8.0 | .NET 8.0 |  19.35 ns |  0.200 ns |  0.132 ns | 1.20x slower |   0.01x |      - |         - |          NA |
|             |          |          |           |           |           |              |         |        |           |             |
| Json_Exists | .NET 9.0 | .NET 9.0 |  16.24 ns |  0.170 ns |  0.101 ns | 1.27x faster |   0.10x |      - |         - |          NA |
| Ini_Exists  | .NET 9.0 | .NET 9.0 |  17.38 ns |  1.008 ns |  0.667 ns | 1.18x faster |   0.10x |      - |         - |          NA |
| Xml_Exists  | .NET 9.0 | .NET 9.0 |  16.21 ns |  0.247 ns |  0.164 ns | 1.27x faster |   0.10x |      - |         - |          NA |
| Yaml_Exists | .NET 9.0 | .NET 9.0 |  16.33 ns |  0.364 ns |  0.241 ns | 1.26x faster |   0.10x |      - |         - |          NA |
| Toml_Exists | .NET 9.0 | .NET 9.0 |  16.63 ns |  0.776 ns |  0.513 ns | 1.24x faster |   0.10x |      - |         - |          NA |
| Json_Get    | .NET 9.0 | .NET 9.0 |  20.55 ns |  2.527 ns |  1.672 ns |     baseline |         |      - |         - |          NA |
| Ini_Get     | .NET 9.0 | .NET 9.0 |  20.48 ns |  3.395 ns |  2.246 ns | 1.00x slower |   0.13x |      - |         - |          NA |
| Xml_Get     | .NET 9.0 | .NET 9.0 |  20.85 ns |  3.526 ns |  2.332 ns | 1.02x slower |   0.13x |      - |         - |          NA |
| Yaml_Get    | .NET 9.0 | .NET 9.0 |  15.57 ns |  0.136 ns |  0.090 ns | 1.32x faster |   0.10x |      - |         - |          NA |
| Toml_Get    | .NET 9.0 | .NET 9.0 |  16.98 ns |  1.316 ns |  0.870 ns | 1.21x faster |   0.11x |      - |         - |          NA |
| Json_GetInt | .NET 9.0 | .NET 9.0 |  69.84 ns |  1.029 ns |  0.538 ns | 3.42x slower |   0.26x | 0.0013 |      64 B |          NA |
| Ini_GetInt  | .NET 9.0 | .NET 9.0 | 100.23 ns |  1.513 ns |  1.001 ns | 4.91x slower |   0.37x | 0.0013 |      64 B |          NA |
| Xml_GetInt  | .NET 9.0 | .NET 9.0 |  91.96 ns | 12.633 ns |  8.356 ns | 4.50x slower |   0.52x | 0.0013 |      64 B |          NA |
| Yaml_GetInt | .NET 9.0 | .NET 9.0 |  98.08 ns |  3.471 ns |  2.296 ns | 4.80x slower |   0.38x | 0.0013 |      64 B |          NA |
| Toml_GetInt | .NET 9.0 | .NET 9.0 |  69.61 ns |  0.831 ns |  0.494 ns | 3.41x slower |   0.26x | 0.0013 |      64 B |          NA |
| Json_Set    | .NET 9.0 | .NET 9.0 |  19.47 ns |  0.167 ns |  0.111 ns | 1.06x faster |   0.08x |      - |         - |          NA |
| Ini_Set     | .NET 9.0 | .NET 9.0 |  19.50 ns |  0.131 ns |  0.086 ns | 1.05x faster |   0.08x |      - |         - |          NA |
| Xml_Set     | .NET 9.0 | .NET 9.0 |  19.51 ns |  0.187 ns |  0.124 ns | 1.05x faster |   0.08x |      - |         - |          NA |
| Yaml_Set    | .NET 9.0 | .NET 9.0 |  19.49 ns |  0.181 ns |  0.120 ns | 1.05x faster |   0.08x |      - |         - |          NA |
| Toml_Set    | .NET 9.0 | .NET 9.0 |  21.32 ns |  2.199 ns |  1.455 ns | 1.04x slower |   0.10x |      - |         - |          NA |
