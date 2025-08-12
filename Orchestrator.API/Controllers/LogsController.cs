using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Logs.Interfaces;
using Orchestrator.Application.Features.Logs.Models;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize] // All actions in this controller require authentication
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpPost]
        [Authorize(Roles = "User,Administrator")]
        public async Task<IActionResult> SubmitLog([FromBody] CreateLogRequest request)
        {
            await _logService.CreateLogAsync(request);
            return Accepted(); // 202 Accepted is appropriate for a fire-and-forget log submission
        }

        [HttpGet]
        [Authorize(Roles = "User,Administrator")]
        public async Task<IActionResult> GetLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? source,
            [FromQuery] string? level,
            [FromQuery] string? hostname,
            [FromQuery] int skip = 0,
            [FromQuery] int top = 50)
        {
            var logs = await _logService.GetLogsAsync(startDate, endDate, source, level, hostname, skip, top);
            return Ok(logs);
        }
    }
}