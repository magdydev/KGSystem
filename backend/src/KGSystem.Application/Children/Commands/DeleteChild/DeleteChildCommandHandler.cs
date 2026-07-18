using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Children.Commands.DeleteChild;

public sealed class DeleteChildCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<DeleteChildCommand, Unit>
{
    public async Task<Unit> Handle(DeleteChildCommand command, CancellationToken cancellationToken)
    {
        var child = await unitOfWork.Children.GetByIdAsync(command.Id, cancellationToken);
        if (child is null)
            throw new NotFoundException(nameof(Domain.Entities.Child), command.Id);

        unitOfWork.Children.Remove(child);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
