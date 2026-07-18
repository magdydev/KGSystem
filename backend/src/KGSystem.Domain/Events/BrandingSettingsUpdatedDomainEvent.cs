using KGSystem.Domain.Common;

namespace KGSystem.Domain.Events;

public sealed record BrandingSettingsUpdatedDomainEvent(int SettingsId, string AppName) : DomainEvent;
