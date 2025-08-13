using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Roles.Interfaces;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;

        }

        [HttpGet]
        public async Task<IActionResult> ListAll(
            [FromQuery] string? name,
            [FromQuery] int skip = 0,
            [FromQuery] int top = 20)
        {
            var roles = await _roleService.GetRolesAsync(name, skip, top);
            return Ok(roles);
        }
    }
}