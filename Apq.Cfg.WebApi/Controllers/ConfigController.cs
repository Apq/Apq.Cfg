using Apq.Cfg.Sources;
using Apq.Cfg.WebApi.Models;
using Apq.Cfg.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.WebApi.Controllers;

/// <summary>
/// 配置 API 控制器
/// </summary>
[ApiController]
[Route("api/apqcfg")]
public class ConfigController : ControllerBase
{
    private readonly IConfigApiService _service;
    private readonly WebApiOptions _options;

    public ConfigController(IConfigApiService service, IOptions<WebApiOptions> options)
    {
        _service = service;
        _options = options.Value;
    }

    // ========== 合并后配置（Merged）==========

    /// <summary>
    /// 获取合并后的所有配置
    /// </summary>
    [HttpGet("merged")]
    public ActionResult<ApiResponse<Dictionary<string, string?>>> GetMerged()
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetMergedConfig();
        return Ok(ApiResponse<Dictionary<string, string?>>.Ok(data));
    }

    /// <summary>
    /// 获取合并后的配置树
    /// </summary>
    [HttpGet("merged/tree")]
    public ActionResult<ApiResponse<ConfigTreeNode>> GetMergedTree()
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetMergedTree();
        return Ok(ApiResponse<ConfigTreeNode>.Ok(data));
    }

    /// <summary>
    /// 获取合并后的单个配置值
    /// </summary>
    [HttpGet("merged/keys/{*key}")]
    public ActionResult<ApiResponse<ConfigValueResponse>> GetMergedValue(string key)
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetMergedValue(key);
        return Ok(ApiResponse<ConfigValueResponse>.Ok(data));
    }

    /// <summary>
    /// 获取合并后的配置节
    /// </summary>
    [HttpGet("merged/sections/{*section}")]
    public ActionResult<ApiResponse<Dictionary<string, string?>>> GetMergedSection(string section)
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetMergedSection(section);
        return Ok(ApiResponse<Dictionary<string, string?>>.Ok(data));
    }

    // ========== 合并前配置（Sources）==========

    /// <summary>
    /// 获取所有配置源列表
    /// </summary>
    [HttpGet("sources")]
    public ActionResult<ApiResponse<List<ConfigSourceInfo>>> GetSources()
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetSources();
        return Ok(ApiResponse<List<ConfigSourceInfo>>.Ok(data));
    }

    /// <summary>
    /// 获取指定配置源内容
    /// </summary>
    [HttpGet("sources/{level:int}/{name}")]
    public ActionResult<ApiResponse<Dictionary<string, string?>>> GetSourceConfig(int level, string name)
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetSourceConfig(level, name);
        if (data == null)
            return NotFound(ApiResponse<Dictionary<string, string?>>.Fail($"Source {level}/{name} not found", "SOURCE_NOT_FOUND"));

        return Ok(ApiResponse<Dictionary<string, string?>>.Ok(data));
    }

    /// <summary>
    /// 获取指定配置源的配置树
    /// </summary>
    [HttpGet("sources/{level:int}/{name}/tree")]
    public ActionResult<ApiResponse<ConfigTreeNode>> GetSourceTree(int level, string name)
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetSourceTree(level, name);
        if (data == null)
            return NotFound(ApiResponse<ConfigTreeNode>.Fail($"Source {level}/{name} not found", "SOURCE_NOT_FOUND"));

        return Ok(ApiResponse<ConfigTreeNode>.Ok(data));
    }

    /// <summary>
    /// 获取指定配置源的单个配置值
    /// </summary>
    [HttpGet("sources/{level:int}/{name}/keys/{*key}")]
    public ActionResult<ApiResponse<ConfigValueResponse>> GetSourceValue(int level, string name, string key)
    {
        if (!_options.AllowRead)
            return Forbid();

        var data = _service.GetSourceValue(level, name, key);
        if (data == null)
            return NotFound(ApiResponse<ConfigValueResponse>.Fail($"Source {level}/{name} not found", "SOURCE_NOT_FOUND"));

        return Ok(ApiResponse<ConfigValueResponse>.Ok(data));
    }

    // ========== 写入操作 ==========

    /// <summary>
    /// 设置配置值
    /// </summary>
    [HttpPut("keys/{*key}")]
    public ActionResult<ApiResponse<bool>> SetValue(string key, [FromBody] string? value)
    {
        if (!_options.AllowWrite)
            return Forbid();

        var success = _service.SetValue(key, value);
        return Ok(ApiResponse<bool>.Ok(success));
    }

    /// <summary>
    /// 设置指定配置源的配置值
    /// </summary>
    [HttpPut("sources/{level:int}/{name}/keys/{*key}")]
    public ActionResult<ApiResponse<bool>> SetSourceValue(int level, string name, string key, [FromBody] string? value)
    {
        if (!_options.AllowWrite)
            return Forbid();

        var success = _service.SetSourceValue(level, name, key, value);
        if (!success)
            return NotFound(ApiResponse<bool>.Fail($"Source {level}/{name} not found or not writable", "SOURCE_NOT_WRITABLE"));

        return Ok(ApiResponse<bool>.Ok(success));
    }

    /// <summary>
    /// 批量更新配置
    /// </summary>
    [HttpPut("batch")]
    public ActionResult<ApiResponse<bool>> BatchUpdate([FromBody] BatchUpdateRequest request)
    {
        if (!_options.AllowWrite)
            return Forbid();

        var success = _service.BatchUpdate(request);
        return Ok(ApiResponse<bool>.Ok(success));
    }

    /// <summary>
    /// 删除配置
    /// </summary>
    [HttpDelete("keys/{*key}")]
    public ActionResult<ApiResponse<bool>> DeleteKey(string key)
    {
        if (!_options.AllowDelete)
            return Forbid();

        var success = _service.DeleteKey(key);
        return Ok(ApiResponse<bool>.Ok(success));
    }

    /// <summary>
    /// 删除指定配置源的配置
    /// </summary>
    [HttpDelete("sources/{level:int}/{name}/keys/{*key}")]
    public ActionResult<ApiResponse<bool>> DeleteSourceKey(int level, string name, string key)
    {
        if (!_options.AllowDelete)
            return Forbid();

        var success = _service.DeleteSourceKey(level, name, key);
        if (!success)
            return NotFound(ApiResponse<bool>.Fail($"Source {level}/{name} not found or not writable", "SOURCE_NOT_WRITABLE"));

        return Ok(ApiResponse<bool>.Ok(success));
    }

    // ========== 管理操作 ==========

    /// <summary>
    /// 保存配置
    /// </summary>
    [HttpPost("save")]
    public async Task<ActionResult<ApiResponse<bool>>> Save()
    {
        if (!_options.AllowWrite)
            return Forbid();

        var success = await _service.SaveAsync();
        return Ok(ApiResponse<bool>.Ok(success));
    }

    /// <summary>
    /// 重新加载配置
    /// </summary>
    [HttpPost("reload")]
    public ActionResult<ApiResponse<bool>> Reload()
    {
        if (!_options.AllowWrite)
            return Forbid();

        var success = _service.Reload();
        return Ok(ApiResponse<bool>.Ok(success));
    }

    /// <summary>
    /// 导出合并后配置
    /// </summary>
    [HttpGet("export/{format}")]
    public ActionResult Export(string format)
    {
        if (!_options.AllowRead)
            return Forbid();

        var content = _service.Export(format);
        var contentType = format.ToLowerInvariant() == "json" ? "application/json" : "text/plain";
        return Content(content, contentType);
    }

    /// <summary>
    /// 导出指定配置源
    /// </summary>
    [HttpGet("sources/{level:int}/{name}/export/{format}")]
    public ActionResult ExportSource(int level, string name, string format)
    {
        if (!_options.AllowRead)
            return Forbid();

        var content = _service.Export(format, level, name);
        var contentType = format.ToLowerInvariant() == "json" ? "application/json" : "text/plain";
        return Content(content, contentType);
    }
}
