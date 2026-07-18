namespace KGSystem.Domain.Common;

/// <summary>
/// Base type for every domain entity. Provides identity, auditing fields,
/// soft-delete support, and a buffer for domain events raised during a
/// business operation (published by the infrastructure layer after SaveChanges).
/// </summary>
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public int Id { get; protected set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
