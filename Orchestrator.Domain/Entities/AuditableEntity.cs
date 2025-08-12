namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// A base class for entities that require auditing of creation and modification events.
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}