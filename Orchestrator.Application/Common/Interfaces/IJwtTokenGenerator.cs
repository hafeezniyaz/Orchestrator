using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Interfaces
{
    /// <summary>
    /// Define a contract for a service that generates token
    /// </summary>
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string username, IEnumerable<string> roles);
    }
}
