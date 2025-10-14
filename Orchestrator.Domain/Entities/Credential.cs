using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    [Table("TMUSCredential")]
    public class Credential : AuditableEntity
    {
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required string Username { get; set; }
        public required string EncryptedSecret { get; set; }
        public Guid AppId { get; set; }
        public bool isEncrypted { get; set; }
    }
}
