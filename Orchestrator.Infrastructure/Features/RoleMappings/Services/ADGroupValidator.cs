using Orchestrator.Application.Features.RoleMappings.Interfaces;
using System.DirectoryServices.AccountManagement;

namespace Orchestrator.Infrastructure.Features.RoleMappings.Services
{
    public class ADGroupValidator : IADGroupValidator
    {
        public bool DoesGroupExist(string groupName)
        {
            try
            {
                // PrincipalContext will automatically connect to the domain the app is running under.
                using var context = new PrincipalContext(ContextType.Domain);

                // Find the group by its name.
                var group = GroupPrincipal.FindByIdentity(context, groupName);

                // If the group object is not null, it was found.
                return group != null;
            }
            catch (PrincipalServerDownException)
            {
                // This will happen if the application server cannot contact the domain controller.
                // In this case, we have to assume validation cannot be completed.
                // Re-throwing is one option, but returning false is safer to prevent invalid entries.
                // For a production system, you would add extensive logging here.
                return false;
            }
        }
    }
}