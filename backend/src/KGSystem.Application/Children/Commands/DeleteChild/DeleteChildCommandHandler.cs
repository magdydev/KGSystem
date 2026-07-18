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

        var enrollments = await unitOfWork.Enrollments.GetByChildAsync(command.Id, cancellationToken);
        foreach (var enrollment in enrollments)
        {
            var payments = await unitOfWork.Payments.GetByEnrollmentAsync(enrollment.Id, cancellationToken);
            foreach (var payment in payments)
                unitOfWork.Payments.Remove(payment);

            unitOfWork.Enrollments.Remove(enrollment);
        }

        var attendances = await unitOfWork.Attendances.GetByChildAsync(command.Id, cancellationToken);
        foreach (var attendance in attendances)
            unitOfWork.Attendances.Remove(attendance);

        unitOfWork.Children.Remove(child);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
