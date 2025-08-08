using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Logs.Interfaces;
using Orchestrator.Application.Features.Logs.Models;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpPost]
        [Authorize(Roles = "User,Administrator")]
        public async Task<IActionResult> CreateLog([FromBody] CreateLogRequest request)
        {
            var logResponse = await _logService.CreateLogAsync(request);
            return CreatedAtAction(nameof(GetLogs), new { id = logResponse.Id }, logResponse);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,View")]
        public async Task<IActionResult> GetLogs(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? source,
            [FromQuery] int skip = 0,
            [FromQuery] int top = 100)
        {
            var logs = await _logService.GetLogsAsync(startDate, endDate, source, skip, top);
            return Ok(logs);
        }
    }
}