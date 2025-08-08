using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Features.Configs.Interfaces;
using Orchestrator.Application.Features.Configs.Models;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistence;
using System.Text.Json;

namespace Orchestrator.Infrastructure.Features.Configs.Services
{
    public class ConfigService : IConfigService
    {
        private readonly OrchestratorDbContext _dbContext;

        public ConfigService(OrchestratorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ConfigResponse?> GetConfigAsync(string configName, Guid? appId)
        {
            var config = await _dbContext.Configs
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null)
            {
                return null;
            }

            var assets = await _dbContext.ConfigAssets
                .Where(a => a.ConfigId == config.Id)
                .ToDictionaryAsync(a => a.Key, a => JsonSerializer.Deserialize<JsonElement>(a.ValueJson));

            return new ConfigResponse
            {
                Id = config.Id,
                Name = config.Name,
                AppId = config.AppId,
                Assets = assets
            };
        }

        public async Task<AssetResponse> SetAssetAsync(string configName, string key, JsonElement value, Guid? appId)
        {
            // Get or create config
            var config = await _dbContext.Configs
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null)
            {
                config = new Config
                {
                    Id = Guid.NewGuid(),
                    Name = configName,
                    AppId = appId
                };
                _dbContext.Configs.Add(config);
                await _dbContext.SaveChangesAsync();
            }

            // Get or create asset
            var asset = await _dbContext.ConfigAssets
                .FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);

            var valueJson = value.GetRawText();

            if (asset == null)
            {
                asset = new ConfigAsset
                {
                    Id = Guid.NewGuid(),
                    ConfigId = config.Id,
                    Key = key,
                    ValueJson = valueJson
                };
                _dbContext.ConfigAssets.Add(asset);
            }
            else
            {
                asset.ValueJson = valueJson;
            }

            await _dbContext.SaveChangesAsync();

            return new AssetResponse
            {
                Id = asset.Id,
                ConfigId = asset.ConfigId,
                Key = asset.Key,
                Value = value
            };
        }

        public async Task<bool> DeleteAssetAsync(string configName, string key, Guid? appId)
        {
            var config = await _dbContext.Configs
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null)
            {
                return false;
            }

            var asset = await _dbContext.ConfigAssets
                .FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);

            if (asset == null)
            {
                return false;
            }

            _dbContext.ConfigAssets.Remove(asset);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteConfigAsync(string configName, Guid? appId)
        {
            var config = await _dbContext.Configs
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null)
            {
                return false;
            }

            // Remove all assets first
            var assets = await _dbContext.ConfigAssets
                .Where(a => a.ConfigId == config.Id)
                .ToListAsync();

            _dbContext.ConfigAssets.RemoveRange(assets);

            // Remove config
            _dbContext.Configs.Remove(config);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}