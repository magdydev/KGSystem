using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.ReferenceData.KGPhases.Commands.UpdateKGPhase;

public sealed record UpdateKGPhaseCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    int SortOrder,
    string? DescriptionAr,
    string? DescriptionEn) : ICommand<Unit>;
