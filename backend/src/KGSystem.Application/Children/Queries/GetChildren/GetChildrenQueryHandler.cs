using AutoMapper;
using KGSystem.Application.Children.Dtos;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Children.Queries.GetChildren;

public sealed class GetChildrenQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetChildrenQuery, IReadOnlyList<ChildDto>>
{
    public async Task<IReadOnlyList<ChildDto>> Handle(GetChildrenQuery query, CancellationToken cancellationToken)
    {
        ChildStatus? status = query.Status switch
        {
            "Active" => ChildStatus.Active,
            "Inactive" => ChildStatus.Inactive,
            "Transferred" => ChildStatus.Transferred,
            "Graduated" => ChildStatus.Graduated,
            _ => null
        };

        var children = await unitOfWork.Children.SearchAsync(query.SearchTerm, status, query.PhaseId, cancellationToken);

        return mapper.Map<IReadOnlyList<ChildDto>>(children);
    }
}
