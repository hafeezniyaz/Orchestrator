using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// Represents a versioned, deployable package associated with an App.
    /// </summary>
    [Table("TMUSPackage")]
    public class Package : AuditableEntity
    {
        public Guid AppId { get; set; }
        public required string Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime UploadedDate { get; set; }
        public required string FilePath { get; set; }
        public required string Md5Checksum { get; set; }
    }
}
