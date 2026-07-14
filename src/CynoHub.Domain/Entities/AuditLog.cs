namespace CynoHub.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
