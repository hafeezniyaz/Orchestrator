using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Roles.Interfaces;

namespace Orchestrator.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Roles ="Admin")]
    public class RolesController:ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        #region GET
        [HttpGet]
        public async Task<IActionResult> ListAllRoles(
    [FromQuery] string? name,
    [FromQuery] int skip = 0,
    [FromQuery] int top = 50
    )
        {
            var roles = await _roleService.GetRolesAsync(name, skip, top);
            return Ok(roles);
        } 
        #endregion
    }
}
