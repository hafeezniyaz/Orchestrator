using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Logs.Interfaces;
using Orchestrator.Application.Features.Logs.Models;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogsController(ILogService logService)
        {
            _logService = logService;
        }


        #region GET
        [HttpGet]
        public async Task<IActionResult> GetLogs
    (
    [FromQuery] DateTime? stateDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] string? source,
    [FromQuery] string? level,
    [FromQuery] string? hostname,
    [FromQuery] int skip = 0,
    [FromQuery] int top = 50
    )
        {
            var logs = await _logService.GetLogsAsync(stateDate, endDate, source, level, hostname, skip, top);
            return Ok(logs);
        }
        #endregion

        #region POST
        [HttpPost]
        public async Task<IActionResult> SubmitLog([FromBody] CreateLogRequest request)
        {
            await _logService.CreateLogAsync(request);
            return Accepted();
        }

        #endregion

    }
}
