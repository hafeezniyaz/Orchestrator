using Microsoft.EntityFrameworkCore;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
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
    }
}