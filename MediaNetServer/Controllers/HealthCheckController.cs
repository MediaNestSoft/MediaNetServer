using MediaNetServer.Data.media.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("health")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;
    private readonly MediaContext                 _dbContext;

    public HealthCheckController(
        ILogger<HealthCheckController> logger,
        MediaContext dbContext)
    {
        _logger    = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealthStatus()
    {
        bool   dbOk;
        string dbMsg;

        try
        {
            // 仅检查数据库连通性
            dbOk  = await _dbContext.Database.CanConnectAsync();
            dbMsg = dbOk ? "connected" : "unreachable";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            dbOk  = false;
            dbMsg = $"error: {ex.Message}";
        }

        // 构造返回的 DTO，仅包含数据库状态
        var response = new HealthResponse(
            isHealthy: new Option<bool?>(dbOk),
            message: new Option<string?>(dbOk ? "Database connected" : "Database unreachable")
        );

        if (!dbOk)
        {
            // 数据库不可用时返回 503
            return StatusCode(503, response);
        }

        // 正常时返回 200
        return Ok(response);
    }
}