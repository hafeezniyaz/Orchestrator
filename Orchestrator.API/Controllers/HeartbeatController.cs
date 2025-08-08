using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Heartbeat.Interfaces;
using Orchestrator.Application.Features.Heartbeat.Models;

namespace Orchestrator.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // Any authenticated client can send a heartbeat.
public class HeartbeatController : ControllerBase
{
    private readonly IHeartbeatService _heartbeatService;

    public HeartbeatController(IHeartbeatService heartbeatService)
    {
        _heartbeatService = heartbeatService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] HeartbeatRequest request)
    {
        await _heartbeatService.RecordHeartbeatAsync(request);
        return Ok();
    }
}