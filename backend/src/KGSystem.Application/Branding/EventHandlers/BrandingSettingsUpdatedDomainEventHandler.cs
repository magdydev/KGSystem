using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Events;
using Microsoft.Extensions.Logging;

namespace KGSystem.Application.Branding.EventHandlers;

public sealed class BrandingSettingsUpdatedDomainEventHandler(ILogger<BrandingSettingsUpdatedDomainEventHandler> logger)
    : IDomainEventHandler<BrandingSettingsUpdatedDomainEvent>
{
    public Task Handle(BrandingSettingsUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Domain event handled: branding settings updated to '{AppName}' at {OccurredOn}",
            domainEvent.AppName,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}
