using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Orchestrator.API.Models;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Application.Features.RoleMappings.Models;
using System.Web;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/ad-groups")]
    [Authorize(Roles ="Admin")]
    public class ADGroupController : ControllerBase
    {
        private IRoleMappingSevice _roleMappingService;
        private IADGroupValidator _adGroupValidator;
        public ADGroupController(IRoleMappingSevice roleMappingSevice, IADGroupValidator adGroupValidator)
        {
            _roleMappingService = roleMappingSevice;
            _adGroupValidator = adGroupValidator;
        }

        #region GET
        [HttpGet]
        public async Task<IActionResult> GetGroupsWithRoles()
        {
            var mappings = await _roleMappingService.GetADRoleMappingsAsync();

            return Ok(mappings);
        }
        #endregion

        #region PUT
        [HttpPut("{adGroupName}")]
        public async Task<IActionResult> SetAdGroupRole(string adGroupName, [FromBody] SetADGroupRoleRequest request)
        {
            //var decodedGroupName = HttpUtility.UrlDecode(adGroupName);

            if (!_adGroupValidator.DoesGroupExist(adGroupName))
            {
                return BadRequest(new ErrorResponse { Error = "invalid_group", ErrorDesciption = $"Group {adGroupName} is invalid" });
            }

            var invalidRoles = await _roleMappingService.FindInvalidRolesAsync(new[] { request.RoleName });

            if (invalidRoles.Any())
            {
                return BadRequest(new ErrorResponse { Error = "invalid_roles", ErrorDesciption = $"role {request.RoleName} is invalid" });

            }

            await _roleMappingService.SetADGroupRoleAsync(adGroupName, request.RoleName);
            return NoContent();
        }
        #endregion

        #region DELETE
        [HttpDelete("{adGroupName}")]
        public async Task<IActionResult> RemoveAdGroupMapping(string adGroupName)
        {
            //var decodedGroupName = HttpUtility.UrlDecode(adGroupName);
            await _roleMappingService.RemoveADGroupRoleAsync(adGroupName);
            return NoContent();

        } 
        #endregion

    }
}
