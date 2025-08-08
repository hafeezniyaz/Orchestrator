using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Apps.Models
{
    public class AppDto
    {
        public Guid Id { get; set; }
        public required string  Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
