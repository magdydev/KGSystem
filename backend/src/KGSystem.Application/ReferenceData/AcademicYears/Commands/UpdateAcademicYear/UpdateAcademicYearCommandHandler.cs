using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.UpdateAcademicYear;

public sealed class UpdateAcademicYearCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateAcademicYearCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAcademicYearCommand command, CancellationToken cancellationToken)
    {
        var year = await unitOfWork.AcademicYears.GetByIdAsync(command.Id, cancellationToken);
        if (year is null)
            throw new NotFoundException(nameof(Domain.Entities.AcademicYear), command.Id);

        year.Update(command.NameAr, command.NameEn, command.StartDate, command.EndDate);

        if (command.IsActive)
            year.Activate();
        else
            year.Deactivate();

        unitOfWork.AcademicYears.Update(year);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
