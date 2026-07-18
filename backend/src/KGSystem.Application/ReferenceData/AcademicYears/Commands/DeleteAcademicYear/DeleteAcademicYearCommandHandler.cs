using KGSystem.Application.Common;
using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Exceptions;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.DeleteAcademicYear;

public sealed class DeleteAcademicYearCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<DeleteAcademicYearCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAcademicYearCommand command, CancellationToken cancellationToken)
    {
        var year = await unitOfWork.AcademicYears.GetByIdAsync(command.Id, cancellationToken);
        if (year is null)
            throw new NotFoundException(nameof(AcademicYear), command.Id);

        if (!year.IsActive)
            throw new DomainException("Cannot delete an archived academic year.");

        var enrollments = await unitOfWork.Enrollments.GetByAcademicYearAsync(command.Id, cancellationToken);
        if (enrollments.Count > 0)
            throw new DomainException("Cannot delete an academic year that has enrollments.");

        var fees = await unitOfWork.MonthlyFees.GetByYearAsync(command.Id, cancellationToken);
        foreach (var fee in fees)
            unitOfWork.MonthlyFees.Remove(fee);

        unitOfWork.AcademicYears.Remove(year);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
