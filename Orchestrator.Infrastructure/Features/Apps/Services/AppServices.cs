using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Orchestrator.Application.Common.Models;
using Orchestrator.Application.Features.Apps.Interfaces;
using Orchestrator.Application.Features.Apps.Models;
using Orchestrator.Infrastructure.Features.Apps.Helpers;
using Orchestrator.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Apps.Services
{
    public class AppServices : IAppService
    {
        private readonly OrchestratorDbContext _context;
        private readonly ILogger<AppServices> _logger;

        public AppServices(OrchestratorDbContext context, ILogger<AppServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AppDto?> CreateAppAsync(CreateAppRequest request)
        {
            var appWithSameName = await _context.Apps.FirstOrDefaultAsync(a => a.Name == request.Name);

            if (appWithSameName != null) return null;

            var app = new Domain.Entities.App
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                Id = Guid.NewGuid()
            };

            await _context.Apps.AddAsync(app);

            return app.ToAppDto();
        }

        public async Task<bool> DeleteAppAsync(Guid id)
        {
            var appToDelete = await _context.Apps
                .FindAsync(id); //cascade delete is taken care in dbcontext

            if (appToDelete == null)
            {
                return false;
            }

            _context.Apps.Remove(appToDelete);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<AppDto>> GetAllAppsAsync(string? name, bool? isActive, int skip = 0, int top = 50)
        {

            var query = _context.Apps.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(a => a.Name.Contains(name));

            if (isActive.HasValue)
                query = query.Where(a => a.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.Name)
                .Skip(skip)
                .Take(top)
                 .Select(app => new AppDto
                 {
                     Id = app.Id,
                     Name = app.Name,
                     Description = app.Description,
                     IsActive = app.IsActive
                 }).ToListAsync();

            return new PagedResult<AppDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<AppDto?> GetAppByIdAsync(Guid appId)
        {
            var app = await _context.Apps.FindAsync(appId);
            if (app == null)
            {
                return null;
            }

            return app.ToAppDto();
        }

        public async Task<AppDto?> UpdateAppAsync(Guid id, UpdateAppRequest request)
        {
            var existingApp = await _context.Apps.FindAsync(id);

            if (existingApp == null)
            {
                return null;
            }

            existingApp.Name = request.Name;
            existingApp.Description = request.Description;
            existingApp.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return existingApp.ToAppDto();
        }
    }
}
