using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Children.Commands.CreateChild;

public sealed class CreateChildCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateChildCommand, Guid>
{
    public async Task<Guid> Handle(CreateChildCommand command, CancellationToken cancellationToken)
    {
        var gender = Enum.Parse<Gender>(command.Gender);

        var child = new Child(
            command.FirstNameAr,
            command.FirstNameEn,
            command.LastNameAr,
            command.LastNameEn,
            command.DateOfBirth,
            gender,
            command.GuardianNameAr,
            command.GuardianNameEn,
            command.GuardianPhone,
            command.GuardianEmail);

        await unitOfWork.Children.AddAsync(child, cancellationToken);

        if (command.KGPhaseId.HasValue && command.AcademicYearId.HasValue)
        {
            var phase = await unitOfWork.KGPhases.GetByIdAsync(command.KGPhaseId.Value, cancellationToken);
            if (phase is null)
                throw new NotFoundException(nameof(KGPhase), command.KGPhaseId.Value);

            var year = await unitOfWork.AcademicYears.GetByIdAsync(command.AcademicYearId.Value, cancellationToken);
            if (year is null)
                throw new NotFoundException(nameof(AcademicYear), command.AcademicYearId.Value);

            var enrollment = new Enrollment(child.Id, command.KGPhaseId.Value, command.AcademicYearId.Value);
            await unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return child.Id;
    }
}
