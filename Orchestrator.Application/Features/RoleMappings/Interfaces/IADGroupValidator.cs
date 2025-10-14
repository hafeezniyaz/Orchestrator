using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.RoleMappings.Interfaces
{
    public interface IADGroupValidator
    {
        /// <summary>
        /// check if the AD group exist
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool DoesGroupExist(string groupName);
    }
}
