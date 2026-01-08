# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/).

## [1.2.0] - 2026-01-09

### Breaking Changes

- **File Configuration Source Method Renaming**: To align with `Microsoft.Extensions.Configuration` API naming conventions, file-based configuration source methods have been renamed:
  - `AddJson` → `AddJsonFile`
  - `AddIni` → `AddIniFile`
  - `AddXml` → `AddXmlFile`
  - `AddYaml` → `AddYamlFile`
  - `AddToml` → `AddTomlFile`
  - `AddEnv` → `AddEnvFile`

### Migration Guide

```csharp
// Old code
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddYaml("config.yaml")
    .AddEnv(".env")
    .Build();

// New code
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml")
    .AddEnvFile(".env")
    .Build();
```

## [1.1.8] - 2026-01-09

### Added

- **WebUI Add Configuration**: Support adding new configuration items in writable sources, including root-level configurations
- **Read-only Source Indicator**: Display read-only badge in source list to distinguish writable and read-only sources

### Changed

- **Chinese Content Optimization**: JSON files now save Chinese characters directly instead of Unicode escape sequences
- **WebUI Interface Improvements**:
  - Configuration source sidebar supports collapse/expand
  - Separated refresh button for clearer operations
  - Leaf nodes with values no longer show add button
- **Authentication Fix**: Fixed WebApi authentication issues

## [1.1.7] - 2026-01-06

### Added

- **CSV Export Format**: Import/export functionality now supports CSV format
- **Drag & Drop Import**: Support drag and drop files for configuration import
- **WebApiDemo Sample**: New complete WebApi usage sample project

### Changed

- **Project Structure Reorganization**: Split into two solutions (core library and WebUI) for optimized build process
- **WebUI Architecture**: Changed to pure static site for simplified deployment
- **CORS Configuration**: Automatic CORS configuration when adding WebApi
- **API Documentation**: Automatically select Swagger or Scalar documentation based on framework version

## [1.1.6] - 2026-01-05

### Changed

- **NuGet Package Dependencies Fix**: Fixed dependency version issues, ensuring correct dependency relationships
- **Build Optimization**: Added `-f net10.0` to specify target framework, optimized build process
- **Package Management**: Set NuGet package list status

## [1.1.5] - 2026-01-04

### Added

- **OpenAPI Support**: WebApi project integrates OpenAPI (Swagger) documentation for easier API debugging and integration
- **Virtual Directory Deployment**: WebUI supports access from any virtual directory without additional configuration; same build artifact can be deployed to different paths

### Changed

- **API Method Renaming**: `GetOrDefault` method renamed to `GetValue` for clearer semantics
- **Build Process Optimization**: Use a text file to centrally manage the list of projects to be packaged
- **Unit Tests**: Test cases increased to 552, improving code coverage
- **Frontend Dependencies Update**: Updated WebUI frontend dependencies to latest versions

## [1.1.4] - 2026-01-03

### Added

- **Web API Project**: New `Apq.Cfg.WebApi` NuGet package providing RESTful API for remote configuration management
- **Web Management UI**: New `Apq.Cfg.WebUI` project providing a web interface for centralized multi-application configuration management (Docker deployment)

### Changed

- **ICfgSource Interface Enhancement**:
  - Added `Name` property (source name, unique within the same level)
  - Added `Type` property (source type name)
  - Added `KeyCount` property (total configuration key count)
  - Added `TopLevelKeyCount` property (top-level key count)
  - Added `GetAllValues()` method (get all configuration values)
- **IWritableCfgSource Interface Refactoring**:
  - Removed `SetValue`, `Remove`, `SaveAsync` methods
  - Added `ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken ct)` method
- **ConfigSourceInfo Class**: Moved to `Apq.Cfg.Sources` namespace
- Updated documentation site with WebApi and WebUI project information
- Updated build scripts to support WebApi project NuGet packaging

## [1.1.3] - 2026-01-03

### Breaking Changes

- **API Simplification**: Removed `Get(string key)` method from `ICfgRoot` and `ICfgSection` interfaces
  - Use indexer `cfg["key"]` or `section["key"]` instead
  - Indexer now supports both read and write operations
- **Method Renaming**:
  - `Set(string key, string? value, int? targetLevel)` → `SetValue(...)`
  - `SetMany(...)` → `SetManyValues(...)`

### Changed

- Simplified `GetValue<T>` implementation by reusing the indexer internally
- Updated source generator (CodeEmitter) to use indexer syntax
- Updated all documentation and code examples to reflect API changes

### Migration Guide

```csharp
// Old code
var value = cfg.Get("Key");
cfg.Set("Key", "Value");
cfg.SetMany(dict);

// New code
var value = cfg["Key"];
cfg.SetValue("Key", "Value");
cfg.SetManyValues(dict);
```

## [1.1.2] - 2026-01-02

### Added

- **Default Levels Feature**: New `CfgSourceLevels` static class defining default levels for each configuration source
  - Local file sources (Json, Ini, Xml, Yaml, Toml): Level 0
  - Remote storage (Redis, Database): Level 100
  - Configuration centers (Consul, Etcd, Nacos, Apollo, Zookeeper): Level 200
  - Secret management (Vault): Level 300
  - Environment-related (Env, EnvironmentVariables): Level 400
- **Indexer Support**: `ICfgSection` now supports indexer access via `section[index]` for array elements
- **Configuration Validation**: Support for multiple validation rules (Required, Range, Regex, OneOf, Length, DependsOn, Custom)
- **Variable Substitution**: Template engine supporting `${Key}`, `${ENV:Name}`, `${SYS:Property}` syntax for configuration value references

### Changed

- Updated all configuration source documentation with default level information
- Updated config-sources and examples directories in both Chinese and English documentation sites
- Code examples now use default levels consistently

## [1.1.1] - 2026-01-01

### Changed
- Fixed .NET version descriptions in documentation
- Removed outdated references to .NET 6.0/7.0/9.0 in documentation

## [1.1.0] - 2026-01-01

### Breaking Changes
- **Target Framework Update**: Now supports only .NET 8.0 and .NET 10.0 (LTS versions)
- Removed .NET 6.0/7.0/9.0 support

### Changed
- Optimized dependency version strategy: `Microsoft.Extensions.*` packages use matching versions per target framework
  - net8.0 → 8.0.0
  - net10.0 → 10.0.1
- Updated performance benchmarks to .NET 10.0

## [1.0.6] - 2025-12-30

### Added
- English documentation support
- SEO optimization (meta tags, sitemap)

### Changed
- Enhanced encryption & masking documentation
- Optimized documentation structure

## [1.0.5] - 2025-12-29

### Added
- Configuration encryption and masking (`Apq.Cfg.Crypto`)
- Multiple encryption algorithms (AES-GCM, AES-CBC, ChaCha20, RSA, SM4)
- Sensitive configuration masking

### Changed
- Performance optimization
- Improved unit test coverage
- Enhanced performance benchmarks

## [1.0.4] - 2025-12-28

### Added
- Nacos configuration source support
- Apollo configuration source support
- Etcd configuration source support
- Zookeeper configuration source support

### Changed
- Optimized hot reload mechanism for remote sources

## [1.0.3] - 2025-12-26

### Added
- Vault configuration source support
- Redis configuration source support
- Database configuration source support

### Changed
- Improved encoding detection accuracy
- Enhanced error messages

## [1.0.2] - 2025-12-24

### Added
- TOML configuration source support
- INI configuration source support
- XML configuration source support
- Configuration change events support

### Changed
- Optimized memory usage
- Improved dynamic reload stability

## [1.0.1] - 2025-12-22

### Added
- Consul configuration source support
- YAML configuration source support
- .env file support
- Configuration section binding support

### Fixed
- Fixed concurrent read issues
- Fixed configuration reload memory leak

## [1.0.0] - 2025-12-22

### Added
- Initial release
- Core configuration library
- JSON configuration source support
- Environment variables support
- Multi-source level-based merging
- Dynamic reload support
- Writable configuration support
- Dependency injection integration
- Source generator support (Native AOT)

## [0.0.1] - 2025-12-20

### Added
- Project initialization
