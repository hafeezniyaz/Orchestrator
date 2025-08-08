namespace Orchestrator.Domain.Entities
{
    /// <summary>
    /// A base class for entities that need audit tracking (who created/modified and when).
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}