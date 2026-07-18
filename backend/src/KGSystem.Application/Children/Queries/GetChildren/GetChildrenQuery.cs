using KGSystem.Application.Children.Dtos;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Children.Queries.GetChildren;

public sealed record GetChildrenQuery(
    string? SearchTerm,
    string? Status,
    Guid? PhaseId) : IQuery<IReadOnlyList<ChildDto>>;
