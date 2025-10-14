using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.Application.Features.Credentials.Interfaces;
using Orchestrator.Application.Features.Credentials.Models;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/apps/{appId}/credentials")]
    public class CredentialController : ControllerBase
    {
        private readonly ICredentialServices _credentialServices;

        public CredentialController(ICredentialServices credentialServices)
        {
            _credentialServices = credentialServices;
        }


        #region GET
        [HttpGet("{credentialName}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetByName
    (
        [FromRoute] string credentialName,
        [FromRoute] Guid appId
    )
        {
            var credentials = await _credentialServices.GetCredentialAsync(appId, credentialName);
            return Ok(credentials);
        }
        #endregion


        #region POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCredentialRequest request,
            [FromRoute] Guid appId)
        {
            var newCredentialDto = await _credentialServices.CreateCredentialAsync(appId, request);

            return CreatedAtAction
                (
                nameof(GetByName), new { appId = newCredentialDto.AppID, credentialName = newCredentialDto.Name },
                newCredentialDto
                );
        }
        #endregion


        #region PUT
        [HttpPut("{credentialName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid appId,
            [FromRoute] string credentialName,
            [FromBody] UpdateCredentialRequest request
            )
        {
            await _credentialServices.UpdateCredentialAsync(appId, credentialName, request);
            return NoContent();
        }
        #endregion


        #region DELETE
        [HttpDelete("{credentialName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(

    [FromRoute] Guid appId,
    [FromRoute] string credentialName
    )
        {
            await _credentialServices.DeleteCredentialAsync(appId, credentialName);
            return NoContent();
        } 
        #endregion

    }
}
