using KGSystem.Domain.Common;

namespace KGSystem.Domain.Events;

public sealed record BrandingSettingsUpdatedDomainEvent(Guid SettingsId, string AppName) : DomainEvent;
