using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Application.Features.RoleMappings.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;

namespace Orchestrator.API.Controllers
{
    // This DTO is specific to the controller for the request body
    public class SetADGroupRoleRequest
    {
        [Required]
        public required string RoleName { get; set; }
    }

    [ApiController]
    [Route("api/v1/role-mappings")]
    [Authorize(Roles = "Administrator")]
    public class RoleMappingsController : ControllerBase
    {
        private readonly IRoleMappingService _roleMappingService;
        private readonly IADGroupValidator _adGroupValidator;

        public RoleMappingsController(IRoleMappingService roleMappingService, IADGroupValidator adGroupValidator)
        {
            _roleMappingService = roleMappingService;
            _adGroupValidator = adGroupValidator;
        }

        [HttpGet("ad-groups")]
        public async Task<IActionResult> GetAdGroupMappings()
        {
            var mappings = await _roleMappingService.GetADGroupRoleMappingsAsync();
            return Ok(mappings);
        }

        [HttpPut("ad-groups/{adGroupName}")]
        public async Task<IActionResult> SetAdGroupRole(string adGroupName, [FromBody] SetADGroupRoleRequest request)
        {
            var decodedGroupName = HttpUtility.UrlDecode(adGroupName);

            // 3. Add the new validation logic
            if (!_adGroupValidator.DoesGroupExist(decodedGroupName))
            {
                return BadRequest($"The Active Directory group '{decodedGroupName}' could not be found.");
            }


            var invalidRoles = await _roleMappingService.FindInvalidRolesAsync(new[] { request.RoleName });
            if (invalidRoles.Any())
            {
                return BadRequest($"The role '{request.RoleName}' is invalid.");
            }

            await _roleMappingService.SetADGroupRoleAsync(decodedGroupName, request.RoleName);
            return NoContent();
        }

        [HttpDelete("ad-groups/{adGroupName}")]
        public async Task<IActionResult> RemoveAdGroupRole(string adGroupName)
        {
            var decodedGroupName = HttpUtility.UrlDecode(adGroupName);
            await _roleMappingService.RemoveADGroupRoleAsync(decodedGroupName);
            return NoContent();
        }
    }
}