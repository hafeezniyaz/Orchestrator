using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orchestrator.Application.Features.RoleMappings.Interfaces;
using Orchestrator.Application.Features.RoleMappings.Models;
using Orchestrator.Domain.Entities;
using Orchestrator.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Features.RoleMappings.Services
{
    public class RoleMappingService : IRoleMappingSevice
    {
        private readonly OrchestratorDbContext _context;

        public RoleMappingService(OrchestratorDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<string>> FindInvalidRolesAsync(IEnumerable<string> roleNames)
        {
            var distinctRoles = roleNames.Distinct().ToList();

            var validRoles = await _context.Roles
                .Where(r => distinctRoles.Contains(r.Name))
                .Select(r => r.Name)
                .ToListAsync();

            return distinctRoles.Except(validRoles);
        }

        public async Task<IEnumerable<ADRoleMappingDto>> GetADRoleMappingsAsync()
        {
            return await _context.RoleMappings
                .AsNoTracking()
                .Where(rm => rm.AdGroup != null)
                .Select(rm => new ADRoleMappingDto { ADGroupName = rm.AdGroup!, RoleName = _context.Roles.First(r => r.Id == rm.RoleId).Name })
                .OrderBy(dto => dto.ADGroupName)
                .ToListAsync();
        }

        public async Task RemoveADGroupRoleAsync(string GroupName)
        {
            var existingMapping = await _context.RoleMappings
                .FirstOrDefaultAsync(rm => rm.AdGroup == GroupName);

            if (existingMapping != null)
            {
                _context.RoleMappings.Remove(existingMapping);
                await _context.SaveChangesAsync();
            }


        }

        public async Task SetADGroupRoleAsync(string GroupName, string roleName)
        {
            var existingMapping = await _context.RoleMappings
                .FirstOrDefaultAsync(rm => rm.AdGroup == GroupName);

            if (existingMapping != null)
            {
                _context.RoleMappings.Remove(existingMapping);
            }

            var role = await _context.Roles.SingleAsync(r => r.Name == roleName);

            var newMapping = new RoleMapping
            {
                RoleId = role.Id,
                AdGroup = GroupName
            };

            _context.RoleMappings.Add(newMapping);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateApiClientRolesAsync(Guid apiClient, IEnumerable<string> roleNames)
        {

            var roleToAssign = await _context.Roles
                 .Where(r => roleNames.Contains(r.Name)).ToListAsync();

            var existingMappings = await _context
                .RoleMappings.Where(r => r.ApiClientId == apiClient)
                .ToListAsync();

            _context.RoleMappings.RemoveRange(existingMappings);

            var newMappings = roleToAssign.Select(r => new RoleMapping
            {
                ApiClientId = apiClient,
                RoleId = r.Id
            });

            await _context.RoleMappings.AddRangeAsync(newMappings);

            await _context.SaveChangesAsync();

        }
    }
}
