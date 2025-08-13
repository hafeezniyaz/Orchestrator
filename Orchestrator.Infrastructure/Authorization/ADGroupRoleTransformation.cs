using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Orchestrator.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Orchestrator.Infrastructure.Authorization
{
    public class ADGroupRoleTransformation : IClaimsTransformation
    {
        // We inject the DbContext directly because this is a transient, scoped service
        // that runs for each authenticated request.
        private readonly OrchestratorDbContext _dbContext;

        public ADGroupRoleTransformation(OrchestratorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not WindowsIdentity windowsIdentity || !windowsIdentity.IsAuthenticated)
            {
                return principal;
            }

            // 1. Get the names of all AD groups the user belongs to.
            var userGroupNames = new List<string>();
            foreach (var group in windowsIdentity.Groups!)
            {
                try
                {
                    userGroupNames.Add(group.Translate(typeof(NTAccount)).Value);
                }
                catch (IdentityNotMappedException)
                {
                    // This is expected and normal for some system SIDs, so we ignore them.
                }
            }

            // 2. Query the database to find roles based on the user's group memberships.
            var rolesFromDb = await _dbContext.RoleMappings
                .AsNoTracking()
                .Where(rm => rm.ADGroup != null && userGroupNames.Contains(rm.ADGroup))
                .Select(rm => _dbContext.Roles.First(r => r.Id == rm.RoleId).Name)
                .Distinct()
                .ToListAsync();

            // 3. Create a new identity, copy existing claims, and add the new roles.
            var newIdentity = new ClaimsIdentity(windowsIdentity.AuthenticationType, windowsIdentity.NameClaimType, windowsIdentity.RoleClaimType);
            newIdentity.AddClaims(principal.Claims);
            foreach (var role in rolesFromDb)
            {
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, role));
            }

            // 4. Fallback: If no roles were found in the DB, assign the default 'User' role.
            if (!rolesFromDb.Any())
            {
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "User"));
            }

            return new ClaimsPrincipal(newIdentity);
        }
    }
}