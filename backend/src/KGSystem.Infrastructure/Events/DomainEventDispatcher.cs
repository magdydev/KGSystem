using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace KGSystem.Infrastructure.Events;

public sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = (IEnumerable<object>)(serviceProvider.GetServices(handlerType) ?? []);
                foreach (dynamic handler in handlers)
                {
                    if (handler is not null) await ((dynamic)handler).Handle((dynamic)domainEvent, cancellationToken);
                }
            }
        }
    }
}
