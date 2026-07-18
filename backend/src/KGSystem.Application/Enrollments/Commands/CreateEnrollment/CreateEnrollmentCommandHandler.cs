using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Enrollments.Commands.CreateEnrollment;

public sealed class CreateEnrollmentCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateEnrollmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var child = await unitOfWork.Children.GetByIdAsync(command.ChildId, cancellationToken);
        if (child is null)
            throw new NotFoundException(nameof(Child), command.ChildId);

        var phase = await unitOfWork.KGPhases.GetByIdAsync(command.KGPhaseId, cancellationToken);
        if (phase is null)
            throw new NotFoundException(nameof(KGPhase), command.KGPhaseId);

        var year = await unitOfWork.AcademicYears.GetByIdAsync(command.AcademicYearId, cancellationToken);
        if (year is null)
            throw new NotFoundException(nameof(AcademicYear), command.AcademicYearId);

        var enrollment = new Enrollment(command.ChildId, command.KGPhaseId, command.AcademicYearId, command.Notes);

        await unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return enrollment.Id;
    }
}
