using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrator.API.Models;
using Orchestrator.Application.Features.Configs.Interfaces;
using System.Text.Json;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/apps/{appId}/configs")]
    [Authorize]
    public class ConfigsController : ControllerBase
    {
        private readonly IConfigServices _configService;

        public ConfigsController(IConfigServices configService)
        {
            _configService = configService;
        }

        #region GET
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListAllConfig([FromRoute] Guid appId, [FromQuery] string? name, int skip = 0, int top = 50)
        {
            var configs = await _configService.ListConfigsForApp(appId, name, skip, top);
            return Ok(configs);
        }

        [HttpGet("{configName}")]
        [Authorize]
        public async Task<IActionResult> GetConfig([FromRoute] Guid appId, string configName)
        {
            var config = await _configService.GetConfigAsync(configName, appId);
            return Ok(config);
        }

        [HttpGet("{configName}/assets/{key}")]
        [Authorize]
        public async Task<IActionResult> GetAsset([FromRoute] Guid appId, string configName, string key)
        {
            var asset = await _configService.GetAssetAsync(configName, key, appId);

            return asset.HasValue ? Ok(asset) : NotFound(new ErrorResponse { Error = "invalid_request", ErrorDesciption = $"{key} was not found in {appId}/{configName}" });

        }
        #endregion

        #region POST
        [HttpPost("{configName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateConfigContainer([FromRoute] Guid appId, string configName)
        {
            await _configService.CreateConfigContainerAsync(configName, appId);
            return CreatedAtAction(nameof(GetConfig), new { appId, configName }, null);
        }
        #endregion

        #region PUT
        [HttpPut("{configName}/asset/{key}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetAsset([FromRoute] Guid appId, string configName, string key, [FromBody] JsonElement value)
        {
            await _configService.SetAssetAsync(configName, key, value, appId);
            return NoContent();
        }
        #endregion

        #region DELETE
        [HttpDelete("{configName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfig([FromRoute] Guid appId, string configName)
        {
            await _configService.DeleteConfigContainerAsync(configName, appId);
            return NoContent();
        }

        [HttpDelete("{configName}/assets/{key}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsset([FromRoute] Guid appId, string configName, string key)
        {
            await _configService.DeleteAssetAsync(configName, key, appId);

            return NoContent();

        } 
        #endregion



    }
}
