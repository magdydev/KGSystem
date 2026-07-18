using KGSystem.Application.Children.Dtos;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Children.Queries.GetChildById;

public sealed record GetChildByIdQuery(Guid Id) : IQuery<ChildDetailDto>;
