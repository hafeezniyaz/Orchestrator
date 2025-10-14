using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;
using System.Security.Claims;
using Orchestrator.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
namespace Orchestrator.Infrastructure.Authorization
{
    public class ADGroupRoleTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _configuartion;
        private readonly OrchestratorDbContext _context;


        public ADGroupRoleTransformation(IConfiguration configuration , OrchestratorDbContext orchestratorDbContext)
        {
            _configuartion = configuration;
            _context = orchestratorDbContext;
        }


        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if(principal.Identity is not WindowsIdentity windowsIdentity || !windowsIdentity.IsAuthenticated)
            {
                return principal;
            }


            //get the group names of the user
            var userGroupNames = new List<string>();

            foreach(var group in windowsIdentity.Groups!)
            {
                try
                {
                    userGroupNames.Add(group.Translate(typeof(NTAccount)).Value);
                }
                catch (IdentityNotMappedException)
                {
                    //ignore unmappable sids
                }
            }

            //get roleMappig from context

            var rolesFromDb = await _context.RoleMappings
                .AsNoTracking()
                .Where(rm => rm.AdGroup != null && userGroupNames.Contains(rm.AdGroup))
                .Select(rm => _context.Roles.First(r => r.Id == rm.RoleId).Name)
                .Distinct()
                .ToListAsync();
            


            var newIdentity = new ClaimsIdentity(windowsIdentity.AuthenticationType, windowsIdentity.NameClaimType, windowsIdentity.RoleClaimType);
            newIdentity.AddClaims(principal.Claims);

            foreach(var role in rolesFromDb)
            {
                  newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, role));

            }

            if (!rolesFromDb.Any())
            {
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "User")); //assign user role by default
            }

            if(windowsIdentity.Name!=null && !newIdentity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                newIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, windowsIdentity.Name));
            }

            return new ClaimsPrincipal(newIdentity);
        }
    }
}
