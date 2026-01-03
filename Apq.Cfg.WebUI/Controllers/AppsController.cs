using Microsoft.AspNetCore.Mvc;
using Apq.Cfg.WebUI.Models;
using Apq.Cfg.WebUI.Services;

namespace Apq.Cfg.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppsController : ControllerBase
{
    private readonly IAppService _appService;

    public AppsController(IAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// 获取所有应用
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AppEndpoint>>> GetAll()
    {
        var apps = await _appService.GetAllAsync();
        return Ok(apps);
    }

    /// <summary>
    /// 获取单个应用
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AppEndpoint>> Get(string id)
    {
        var app = await _appService.GetByIdAsync(id);
        if (app == null) return NotFound();
        return Ok(app);
    }

    /// <summary>
    /// 添加应用
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppEndpoint>> Add([FromBody] AppEndpoint app)
    {
        var created = await _appService.AddAsync(app);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>
    /// 更新应用
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] AppEndpoint app)
    {
        app.Id = id;
        var success = await _appService.UpdateAsync(app);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// 删除应用
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var success = await _appService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    [HttpPost("{id}/test")]
    public async Task<ActionResult<object>> TestConnection(string id)
    {
        var success = await _appService.TestConnectionAsync(id);
        return Ok(new { success });
    }
}
