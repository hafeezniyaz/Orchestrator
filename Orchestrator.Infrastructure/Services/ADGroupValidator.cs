using Orchestrator.Application.Features.RoleMappings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Logging;

namespace Orchestrator.Infrastructure.Services
{
    public class ADGroupValidator : IADGroupValidator
    {
        private readonly ILogger<ADGroupValidator> _logger;

        public ADGroupValidator(ILogger<ADGroupValidator> logger)
        {
            _logger = logger;
        }
        public bool DoesGroupExist(string groupName)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain);

                var group = GroupPrincipal.FindByIdentity(context, groupName);

                return group != null;
            }
            catch
            {
                return false;
            }
        }
    }
}