using Apq.Cfg.Samples.Demos;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              Apq.Cfg 完整功能示例                            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

var baseDir = AppContext.BaseDirectory;

// 基础示例 (1-8)
await BasicUsageDemo.RunAsync(baseDir);
await MultiFormatDemo.RunAsync(baseDir);
await ConfigSectionDemo.RunAsync(baseDir);
await BatchOperationsDemo.RunAsync(baseDir);
await TypeConversionDemo.RunAsync(baseDir);
await DynamicReloadDemo.RunAsync(baseDir);
await DependencyInjectionDemo.RunAsync(baseDir);
await EncodingMappingDemo.RunAsync(baseDir);

// 扩展项目示例 (9-17)
await RedisDemo.RunAsync(baseDir);
await DatabaseDemo.RunAsync(baseDir);
await ConsulDemo.RunAsync(baseDir);
await EtcdDemo.RunAsync(baseDir);
await NacosDemo.RunAsync(baseDir);
await ApolloDemo.RunAsync(baseDir);
await ZookeeperDemo.RunAsync(baseDir);
await VaultDemo.RunAsync(baseDir);
await SourceGeneratorDemo.RunAsync(baseDir);

Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              所有示例执行完成                                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
