using AutoMapper;
using KGSystem.Application.Children.Dtos;
using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Children.Queries.GetChildById;

public sealed class GetChildByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetChildByIdQuery, ChildDetailDto>
{
    public async Task<ChildDetailDto> Handle(GetChildByIdQuery query, CancellationToken cancellationToken)
    {
        var child = await unitOfWork.Children.GetWithEnrollmentsAsync(query.Id, cancellationToken);
        if (child is null)
            throw new NotFoundException(nameof(Domain.Entities.Child), query.Id);

        return mapper.Map<ChildDetailDto>(child);
    }
}
