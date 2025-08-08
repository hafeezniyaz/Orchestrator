using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Features.Apps.Interfaces;
using Orchestrator.Application.Features.Apps.Models;
using Orchestrator.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.Apps.Services
{
    public class AppService : IAppService
    {
        private readonly OrchestratorDbContext _context;

        public AppService(OrchestratorDbContext context)
        {
            _context = context;
        }

        public async Task<AppDto> CreateAppAsync(CreateAppRequest request)
        {
            // 1. Create the domain entity from the request
            var app = new Domain.Entities.App
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            // 2. Add it to the DbContext
            _context.Apps.Add(app);

            // 3. Save changes to the database
            await _context.SaveChangesAsync();

            // 4. Map the new entity back to a DTO and return it
            return new AppDto
            {
                Id = app.Id,
                Name = app.Name,
                Description = app.Description,
                IsActive = app.IsActive
            };
        }

        public async Task<IEnumerable<AppDto>> GetAllAppsAsync()
        {
            return await _context.Apps
             .Select(app => new AppDto
             {
                 Id = app.Id,
                 Name = app.Name,
                 Description = app.Description,
                 IsActive = app.IsActive
             })
             .ToListAsync();
        }

        public async Task<AppDto?> GetAppByIdAsync(Guid appId)
        {
            var app = await _context.Apps.FindAsync(appId);

            if (app == null)
            {
                return null;
            }

            return new AppDto
            {
                Id = app.Id,
                Name = app.Name,
                Description = app.Description,
                IsActive = app.IsActive
            };
        }

        public async Task<AppDto?> UpdateAppAsync(Guid id, UpdateAppRequest request)
        {
            // 1. Find the existing entity in the database
            var existingApp = await _context.Apps.FindAsync(id);

            // 2. If it doesn't exist, return null
            if (existingApp == null)
            {
                return null;
            }

            // 3. Update the properties from the request
            existingApp.Name = request.Name;
            existingApp.Description = request.Description;
            existingApp.IsActive = request.IsActive;

            // 4. Save changes
            await _context.SaveChangesAsync();

            // 5. Map the updated entity to a DTO and return it
            return new AppDto
            {
                Id = existingApp.Id,
                Name = existingApp.Name,
                Description = existingApp.Description,
                IsActive = existingApp.IsActive
            };
        }

        public async Task<bool> DeleteAppAsync(Guid id)
        {
            // 1. Find the existing entity in the database
            var appToDelete = await _context.Apps.FindAsync(id);

            // 2. If it doesn't exist, return false to indicate failure
            if (appToDelete == null)
            {
                return false;
            }

            // 3. Remove the entity from the DbContext
            _context.Apps.Remove(appToDelete);

            // 4. Save changes to the database
            await _context.SaveChangesAsync();

            // 5. Return true to indicate success
            return true;
        }
    }
}
