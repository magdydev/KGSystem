using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.ReferenceData.KGPhases.Commands.CreateKGPhase;

public sealed record CreateKGPhaseCommand(
    string Code,
    string NameAr,
    string NameEn,
    int SortOrder,
    string? DescriptionAr,
    string? DescriptionEn) : ICommand<Guid>;
