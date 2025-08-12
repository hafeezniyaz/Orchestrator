using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Apps.Interfaces;
using Orchestrator.Application.Features.Apps.Models;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize] // All endpoints in this controller require authentication
    public class AppsController : ControllerBase

    {
        private readonly IAppService _appService;

        public AppsController(IAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<IActionResult> ListAll(
            [FromQuery] string? name,
            [FromQuery] bool? isActive,
            [FromQuery] int skip = 0,
            [FromQuery] int top = 50
            )
        {
            var apps = await _appService.GetAllAppsAsync(name, isActive, skip, top);
            return Ok(apps);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var app = await _appService.GetAppByIdAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            return Ok(app);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")] // This endpoint requires the "Administrator" role
        public async Task<IActionResult> Create([FromBody] CreateAppRequest request)
        {
            var newApp = await _appService.CreateAppAsync(request);

            // Return a 201 Created response with a link to the new resource
            return CreatedAtAction(nameof(GetById), new { id = newApp.Id }, newApp);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppRequest request)
        {
            var updatedApp = await _appService.UpdateAppAsync(id, request);

            if (updatedApp == null)
            {
                return NotFound();
            }

            return Ok(updatedApp);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _appService.DeleteAppAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
