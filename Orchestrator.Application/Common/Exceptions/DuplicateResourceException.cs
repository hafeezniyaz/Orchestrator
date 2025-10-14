using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Exceptions
{
    public class DuplicateResourceException : Exception

    {
        public DuplicateResourceException(string message): base(message)
        {}
    }
}
