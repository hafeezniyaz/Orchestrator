using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Features.Credentials.Models
{
    public class CredentialDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Decription { get; set; }
        public required string Username { get; set; }
        public Guid AppID { get; set; }

        public  DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
