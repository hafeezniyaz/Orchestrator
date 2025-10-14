using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Configs.Models
{
    public class ConfigDto
    {
        public required string Name { get; set; }
        public Guid AppId { get; set; }
    }
}
