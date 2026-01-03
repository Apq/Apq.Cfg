using Microsoft.AspNetCore.Mvc;
using Apq.Cfg.WebUI.Services;
using System.Text.Json;

namespace Apq.Cfg.WebUI.Controllers;

/// <summary>
/// 配置 API 代理，解决跨域问题
/// </summary>
[ApiController]
[Route("api/proxy/{appId}")]
public class ProxyController : ControllerBase
{
    private readonly ConfigProxyService _proxyService;

    public ProxyController(ConfigProxyService proxyService)
    {
        _proxyService = proxyService;
    }

    /// <summary>
    /// 代理 GET 请求
    /// </summary>
    [HttpGet("{**path}")]
    public async Task<ActionResult> ProxyGet(string appId, string? path)
    {
        try
        {
            var result = await _proxyService.GetAsync(appId, path);
            return Content(result, "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 代理 PUT 请求
    /// </summary>
    [HttpPut("{**path}")]
    public async Task<ActionResult> ProxyPut(string appId, string? path, [FromBody] JsonElement? body)
    {
        try
        {
            var result = await _proxyService.PutAsync(appId, path, body);
            return Content(result, "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 代理 DELETE 请求
    /// </summary>
    [HttpDelete("{**path}")]
    public async Task<ActionResult> ProxyDelete(string appId, string? path)
    {
        try
        {
            var result = await _proxyService.DeleteAsync(appId, path);
            return Content(result, "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 代理 POST 请求
    /// </summary>
    [HttpPost("{**path}")]
    public async Task<ActionResult> ProxyPost(string appId, string? path, [FromBody] JsonElement? body)
    {
        try
        {
            var result = await _proxyService.PostAsync(appId, path, body);
            return Content(result, "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
