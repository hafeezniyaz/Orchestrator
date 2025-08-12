using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Features.Configs.Interfaces;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Configs.Services
{
    public class ConfigService : IConfigService
    {
        private readonly OrchestratorDbContext _dbContext;

        public ConfigService(OrchestratorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateConfigContainerAsync(string configName, Guid appId )
        {
            var exists = await _dbContext.Configs.AnyAsync(c => c.Name == configName && c.AppId == appId);
            if (!exists)
            {
                _dbContext.Configs.Add(new Config { Name = configName, AppId = appId });
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, JsonElement>> GetConfigAsync(string configName, Guid appId )
        {
            var config = await _dbContext.Configs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null) return new Dictionary<string, JsonElement>();

            return await _dbContext.ConfigAssets
                .AsNoTracking()
                .Where(a => a.ConfigId == config.Id)
                .ToDictionaryAsync(a => a.Key, a => JsonDocument.Parse(a.ValueJson).RootElement);
        }

        public async Task SetAssetAsync(string configName, string key, JsonElement value, Guid appId )
        {
            var config = await _dbContext.Configs
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            // Create the config container if it doesn't exist
            if (config == null)
            {
                config = new Config { Name = configName, AppId = appId };
                _dbContext.Configs.Add(config);
            }

            var asset = await _dbContext.ConfigAssets
                .FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);

            if (asset != null)
            {
                // Update existing asset
                asset.ValueJson = value.GetRawText();
            }
            else
            {
                // Create new asset
                _dbContext.ConfigAssets.Add(new ConfigAsset
                {
                    ConfigId = config.Id,
                    Key = key,
                    ValueJson = value.GetRawText()
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteConfigContainerAsync(string configName, Guid appId)
        {
            var config = await _dbContext.Configs
                .Include(c => _dbContext.ConfigAssets.Where(a => a.ConfigId == c.Id))
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config != null)
            {
                _dbContext.Configs.Remove(config); // EF Core will handle cascading deletes if configured, but removing manually is safer
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAssetAsync(string configName, string key, Guid appId)
        {
            var config = await _dbContext.Configs.AsNoTracking().FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);
            if (config == null) return;

            var asset = await _dbContext.ConfigAssets.FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);
            if (asset != null)
            {
                _dbContext.ConfigAssets.Remove(asset);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<JsonElement?> GetAssetAsync(string configName, string key, Guid appId)
        {
            // Find the config container first
            var config = await _dbContext.Configs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null) return null;

            // Find the specific asset within that container
            var asset = await _dbContext.ConfigAssets
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);

            if (asset == null) return null;

            // Parse the stored JSON string and return it
            return JsonDocument.Parse(asset.ValueJson).RootElement;
        }
    }
}
