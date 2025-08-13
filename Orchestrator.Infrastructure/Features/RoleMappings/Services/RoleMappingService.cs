using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Application.Features.RoleMappings.Models;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistence;

namespace Orchestrator.Infrastructure.Features.RoleMappings.Services
{
    public class RoleMappingService : IRoleMappingService
    {
        private readonly OrchestratorDbContext _dbContext;

        public RoleMappingService(OrchestratorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> FindInvalidRolesAsync(IEnumerable<string> roleNames)
        {
            var distinctRoleNames = roleNames.Distinct().ToList();

            var validRoles = await _dbContext.Roles
                .Where(r => distinctRoleNames.Contains(r.Name))
                .Select(r => r.Name) // We only need the name for comparison
                .ToListAsync();

            return distinctRoleNames.Except(validRoles);
        }

        public async Task UpdateApiClientRolesAsync(Guid apiClientId, IEnumerable<string> roleNames)
        {
            // 1. Find the roles in the database that match the provided names
            var rolesToAssign = await _dbContext.Roles
                .Where(r => roleNames.Contains(r.Name))
                .ToListAsync();

            // 2. Find all existing mappings for this client
            var existingMappings = await _dbContext.RoleMappings
                .Where(rm => rm.ApiClientId == apiClientId)
                .ToListAsync();

            // 3. Remove the old mappings
            _dbContext.RoleMappings.RemoveRange(existingMappings);

            // 4. Create the new mappings
            var newMappings = rolesToAssign.Select(role => new RoleMapping
            {
                RoleId = role.Id,
                ApiClientId = apiClientId
            });

            // 5. Add the new mappings to the context
            await _dbContext.RoleMappings.AddRangeAsync(newMappings);

            // 6. Save all changes (deletions and additions) in one transaction
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ADGroupRoleMappingDto>> GetADGroupRoleMappingsAsync()
        {
            return await _dbContext.RoleMappings
                .AsNoTracking()
                .Where(rm => rm.ADGroup != null)
                .Select(rm => new ADGroupRoleMappingDto
                {
                    ADGroupName = rm.ADGroup!,
                    RoleName = _dbContext.Roles.First(r => r.Id == rm.RoleId).Name
                })
                .OrderBy(dto => dto.ADGroupName)
                .ToListAsync();
        }

        public async Task SetADGroupRoleAsync(string adGroupName, string roleName)
        {
            var existingMapping = await _dbContext.RoleMappings
                .FirstOrDefaultAsync(rm => rm.ADGroup == adGroupName);

            if (existingMapping != null)
            {
                _dbContext.RoleMappings.Remove(existingMapping);
            }

            var role = await _dbContext.Roles.SingleAsync(r => r.Name == roleName);

            var newMapping = new RoleMapping
            {
                RoleId = role.Id,
                ADGroup = adGroupName
            };

            _dbContext.RoleMappings.Add(newMapping);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveADGroupRoleAsync(string adGroupName)
        {
            var existingMapping = await _dbContext.RoleMappings
                .FirstOrDefaultAsync(rm => rm.ADGroup == adGroupName);

            if (existingMapping != null)
            {
                _dbContext.RoleMappings.Remove(existingMapping);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}