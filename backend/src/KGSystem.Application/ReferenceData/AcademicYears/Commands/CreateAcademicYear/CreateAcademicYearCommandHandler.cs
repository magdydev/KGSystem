using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.CreateAcademicYear;

public sealed class CreateAcademicYearCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateAcademicYearCommand, int>
{
    public async Task<int> Handle(CreateAcademicYearCommand command, CancellationToken cancellationToken)
    {
        var year = new AcademicYear(command.Code, command.NameAr, command.NameEn, command.StartDate, command.EndDate, command.IsActive);

        await unitOfWork.AcademicYears.AddAsync(year, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return year.Id;
    }
}
