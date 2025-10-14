using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Configs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Configs.Interfaces
{
    public interface IConfigServices
    {
        /// <summary>
        /// Create config container for an app
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task CreateConfigContainerAsync(string configName, Guid appId);

        /// <summary>
        /// get all assets for a config
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<Dictionary<string, JsonElement>> GetConfigAsync(string configName, Guid appId);

        /// <summary>
        /// set a value for an asset in a config
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task SetAssetAsync(string configName, string key, JsonElement value, Guid appId);

        /// <summary>
        /// Delete config container of an app
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task DeleteConfigContainerAsync(string configName, Guid appId);

        /// <summary>
        /// Delete an asset in a config
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="key"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task DeleteAssetAsync(string configName, string key, Guid appId);

        /// <summary>
        /// get specific asset from a config
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="key"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<JsonElement?> GetAssetAsync(string configName, string key, Guid appId);

        /// <summary>
        /// List all config for the app
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<PagedResult<ConfigDto>> ListConfigsForApp(Guid appId, string? ConfigName, int skip = 0, int top = 50);

    }
}
