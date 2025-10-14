using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Orchestrator.Application.Features.ApiClient.Interfaces;
using Orchestrator.Application.Features.ApiClient.Models;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Orchestrator.API.Models;
using System.Runtime.InteropServices;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/clients")]
    [Authorize(Roles = "Admin")]
    public class ApiClientController : ControllerBase
    {
        private readonly IApiClientService _apiClientService;
        private readonly IRoleMappingSevice _roleMappingSevice;

        public ApiClientController(IApiClientService apiClientService, IRoleMappingSevice roleMappingService)
        {
            _apiClientService = apiClientService;
            _roleMappingSevice = roleMappingService;
        }

        #region GET
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? name, int skip = 0, int top = 50)
        {
            var apiClients = await _apiClientService.ListAllApiClients(name, skip, top);

            return Ok(apiClients);
        }

        #endregion

        #region POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiClientRequest request)
        {
            var invalidRoles = await _roleMappingSevice.FindInvalidRolesAsync(request.Roles);
            if (invalidRoles.Any())
            {
                return BadRequest(new ErrorResponse { Error = "invalid_roles", ErrorDesciption = $"the following roles are invalid: {string.Join(",", invalidRoles)}" });
            }
            var response = await _apiClientService.CreateApiClient(request);
            return CreatedAtAction(nameof(Create), new { id = response.ClientId }, response);
        }
        #endregion

        #region PUT

        [HttpPut("{apiClientId}/roles")]
        public async Task<IActionResult> UpdateApiClientRoles(Guid apiClientId, [FromBody] UpdateApiClientRoleRequest request)
        {
            var invalidRoles = await _roleMappingSevice.FindInvalidRolesAsync(request.Roles);
            if (invalidRoles.Any())
            {
                return BadRequest(new ErrorResponse { Error = "invalid_roles", ErrorDesciption = $"the following roles are invalid: {string.Join(",", invalidRoles)}" });
            }

            await _roleMappingSevice.UpdateApiClientRolesAsync(apiClientId, request.Roles);
            return NoContent();
        }
        #endregion

        #region DELETE
        [HttpDelete]
        public async Task<IActionResult> DeleteClient([FromQuery] Guid clientId)
        {
            await _apiClientService.DeleteClient(clientId);
            return NoContent();
        } 
        #endregion

    }
}
