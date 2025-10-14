using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        /// <summary>
        /// Name of the current authenticated user, can be null if not authenticated
        /// </summary>
         string? UserName {get; }
    }
}
