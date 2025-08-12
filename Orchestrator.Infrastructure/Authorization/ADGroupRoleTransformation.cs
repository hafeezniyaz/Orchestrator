using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Principal;

namespace Orchestrator.Infrastructure.Authorization
{
    public class ADGroupRoleTransformation : IClaimsTransformation
    {
        private readonly IConfiguration _configuration;

        public ADGroupRoleTransformation(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not WindowsIdentity windowsIdentity)
            {
                return Task.FromResult(principal);
            }

            var roleMappings = _configuration.GetSection("AuthorizationSettings:RoleMappings")
                                             .Get<Dictionary<string, string>>();

            if (roleMappings == null || !roleMappings.Any())
            {
                return Task.FromResult(principal);
            }

            var newIdentity = new ClaimsIdentity(windowsIdentity.AuthenticationType, windowsIdentity.NameClaimType, windowsIdentity.RoleClaimType);
            newIdentity.AddClaims(principal.Claims);

            // ---- CHANGE 1: Add a flag to track if we assigned a role. ----
            var roleWasAssigned = false;

            foreach (var group in windowsIdentity.Groups)
            {
                try
                {
                    string groupName = group.Translate(typeof(NTAccount)).Value;

                    if (roleMappings.TryGetValue(groupName, out var role))
                    {
                        newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, role));
                        // ---- CHANGE 2: Set the flag to true since we found a match. ----
                        roleWasAssigned = true;
                    }
                }
                catch (IdentityNotMappedException)
                {
                    // Ignore unmappable SIDs
                }
            }

            // ---- CHANGE 3: If after checking all groups, no role was assigned, give them the default role. ----
            if (!roleWasAssigned)
            {
                newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "User"));
            }

            return Task.FromResult(new ClaimsPrincipal(newIdentity));
        }
    }
}