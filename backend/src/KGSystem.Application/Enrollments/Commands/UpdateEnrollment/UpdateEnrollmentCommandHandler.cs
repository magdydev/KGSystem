using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Enrollments.Commands.UpdateEnrollment;

public sealed class UpdateEnrollmentCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateEnrollmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(command.Id, cancellationToken);
        if (enrollment is null)
            throw new NotFoundException(nameof(Domain.Entities.Enrollment), command.Id);

        enrollment.UpdateNotes(command.Notes ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(command.Status))
        {
            switch (command.Status)
            {
                case "Withdrawn":
                    enrollment.Withdraw();
                    break;
                case "Completed":
                    enrollment.Complete();
                    break;
            }
        }

        unitOfWork.Enrollments.Update(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
