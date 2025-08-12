using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Configs.Interfaces
{
    public interface IConfigService
    {
        /// <summary>
        /// Creates a new configuration container.
        /// </summary>
        /// <param name="configName">The name of the configuration container.</param>
        /// <param name="appId">Optional AppId to scope the configuration to a specific application.</param>
        Task CreateConfigContainerAsync(string configName, Guid appId );

        /// <summary>
        /// Retrieves all assets within a specific configuration container.
        /// </summary>
        /// <returns>A dictionary of key-value pairs.</returns>
        Task<Dictionary<string, JsonElement>> GetConfigAsync(string configName, Guid appId );

        /// <summary>
        /// Creates or updates a specific configuration asset (key-value pair).
        /// </summary>
        /// <param name="key">The key of the asset.</param>
        /// <param name="value">The JSON value of the asset.</param>
        Task SetAssetAsync(string configName, string key, JsonElement value, Guid appId);

        /// <summary>
        /// Deletes an entire configuration container and all its assets.
        /// </summary>
        Task DeleteConfigContainerAsync(string configName, Guid appId);

        /// <summary>
        /// Deletes a specific asset from a configuration container.
        /// </summary>
        Task DeleteAssetAsync(string configName, string key, Guid appId);

        /// <summary>
        /// Retrieves a single asset by its key from within a configuration container.
        /// </summary>
        /// <returns>The JSON value of the asset, or null if not found.</returns>
        Task<JsonElement?> GetAssetAsync(string configName, string key, Guid appId);

    }
}
