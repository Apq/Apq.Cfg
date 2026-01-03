using Apq.Cfg.WebUI.Models;

namespace Apq.Cfg.WebUI.Services;

/// <summary>
/// 应用管理服务接口
/// </summary>
public interface IAppService
{
    Task<List<AppEndpoint>> GetAllAsync();
    Task<AppEndpoint?> GetByIdAsync(string id);
    Task<AppEndpoint> AddAsync(AppEndpoint app);
    Task<bool> UpdateAsync(AppEndpoint app);
    Task<bool> DeleteAsync(string id);
    Task<bool> TestConnectionAsync(string id);
}
