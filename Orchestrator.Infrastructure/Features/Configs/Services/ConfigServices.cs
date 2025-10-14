using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Configs.Interfaces;
using Orchestrator.Application.Features.Configs.Models;
using Orchestrator.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Configs.Services
{
    public class ConfigServices : IConfigServices
    {
        private readonly OrchestratorDbContext _context;
        private readonly ILogger<ConfigServices> _logger;
        
        public ConfigServices(OrchestratorDbContext context , ILogger<ConfigServices> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task CreateConfigContainerAsync(string configName, Guid appId)
        {
            var exists = await _context.Configs.AnyAsync(c => c.Name == configName && c.AppId == appId);
            if (!exists)
            {
                _context.Configs.Add(new Domain.Entities.Config { AppId = appId, Name = configName });
                await _context.SaveChangesAsync();
            }

            _logger.LogWarning($"Create config request failed due to config {configName} already exist for the app {appId}");
        }

        public async Task DeleteAssetAsync(string configName, string key, Guid appId)
        {
            var config = await _context.Configs.AsNoTracking().FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null) return ;

            var asset = await _context.ConfigAssets.FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);

            if (asset != null)
            {
                _context.ConfigAssets.Remove(asset);
                await _context.SaveChangesAsync();
            }

        }

        public async Task DeleteConfigContainerAsync(string configName, Guid appId)
        {
            var config = await _context.Configs
                //.Include(c => _context.ConfigAssets.Where(a => a.ConfigId == c.Id)) //cascading delete de 
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);
            if(config!=null)
            {
                _context.Configs.Remove(config);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<JsonElement?> GetAssetAsync(string configName, string key, Guid appId)
        {
            var config = await _context.Configs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null) return null;

            var asset = await _context.ConfigAssets
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);
            if (asset == null) return null;

            return JsonDocument.Parse(asset.ValueJson).RootElement;
        }

        public async Task<Dictionary<string, JsonElement>> GetConfigAsync(string configName, Guid appId)
        {
            var config = await _context.Configs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null) return new Dictionary<string, JsonElement>();

            return await _context.ConfigAssets
                .AsNoTracking()
                .Where(a => a.ConfigId == config.Id)
                .ToDictionaryAsync(a => a.Key, a => JsonDocument.Parse(a.ValueJson).RootElement);

        }

        public async Task<PagedResult<ConfigDto>> ListConfigsForApp(Guid appId, string? ConfigName, int skip = 0, int top = 50)
        {
            var query =  _context.Configs.Where(c => c.AppId == appId);

            if (!string.IsNullOrEmpty(ConfigName))
                query = query.Where(c => c.Name.Contains(ConfigName));

            var totalCount = await query.CountAsync();

            var configs = await query
                .Skip(skip)
                .Take(top)
                .Select(config => new ConfigDto { AppId = config.AppId, Name = config.Name })
                .ToListAsync();

            return new PagedResult<ConfigDto> { Items = configs, TotalCount = totalCount };

        }

        public async Task SetAssetAsync(string configName, string key, JsonElement value, Guid appId)
        {
            var config = await _context.Configs
               .AsNoTracking()
               .FirstOrDefaultAsync(c => c.Name == configName && c.AppId == appId);

            if (config == null)
            {
                config = new Domain.Entities.Config { Name = configName, AppId = appId };

                _context.Configs.Add(config);
            }

            var asset = await _context.ConfigAssets.FirstOrDefaultAsync(a => a.ConfigId == config.Id && a.Key == key);

            if (asset != null)
            {
                asset.ValueJson = value.GetRawText();
            }

            else
            {
                _context.ConfigAssets.Add(new Domain.Entities.ConfigAsset
                {
                    ConfigId = config.Id,
                    Key = key,
                    ValueJson = value.GetRawText()
                });
            }

            await _context.SaveChangesAsync();

        }
    }
}
