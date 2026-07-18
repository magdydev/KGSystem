using KGSystem.Domain.Common;

namespace KGSystem.Application.Common.Interfaces;

/// <summary>Implement one of these per domain event a feature needs to react to.</summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
}
