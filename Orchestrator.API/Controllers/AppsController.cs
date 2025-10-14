using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.API.Models;
using Orchestrator.Application.Features.Apps.Interfaces;
using Orchestrator.Application.Features.Apps.Models;
using System.Collections.Specialized;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class AppsController : ControllerBase
    {
        private readonly IAppService _appService;

        public AppsController(IAppService appService)
        {
            _appService = appService;

        }

        #region GET
        [HttpGet]
        [Authorize]
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
        public async Task<IActionResult> GetAppById(Guid id)
        {
            var app = await _appService.GetAppByIdAsync(id);
            if (app == null)
                return NotFound();
            return Ok(app);

        }
        #endregion

        #region POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateAppRequest request)
        {
            var newApp = await _appService.CreateAppAsync(request);

            if (newApp == null)
            {
                return BadRequest(new ErrorResponse { Error = "duplicate_name", ErrorDesciption = $"App with name {request.Name} already exist" });
            }

            return CreatedAtAction(nameof(GetAppById), new { id = newApp.Id }, newApp);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppRequest request)
        {
            var updatedApp = await _appService.UpdateAppAsync(id, request);

            if (updatedApp == null)
            {
                return NotFound();
            }
            return Ok(updatedApp);
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var sucess = await _appService.DeleteAppAsync(id);
            if (!sucess)
                return NotFound();

            return NoContent();
        } 
        #endregion
    }
}
