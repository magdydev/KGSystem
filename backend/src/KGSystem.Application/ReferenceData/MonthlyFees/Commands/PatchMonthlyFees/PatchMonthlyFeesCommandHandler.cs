using KGSystem.Application.Common;
using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Exceptions;
using KGSystem.Domain.Repositories;
using KGSystem.Domain.ValueObjects;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Commands.PatchMonthlyFees;

public sealed class PatchMonthlyFeesCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<PatchMonthlyFeesCommand, Unit>
{
    public async Task<Unit> Handle(PatchMonthlyFeesCommand command, CancellationToken cancellationToken)
    {
        var year = await unitOfWork.AcademicYears.GetByIdAsync(command.AcademicYearId, cancellationToken);
        if (year is null)
            throw new NotFoundException(nameof(AcademicYear), command.AcademicYearId);
        if (!year.IsActive)
            throw new DomainException("Cannot modify monthly fees for an archived academic year.");

        var branding = await unitOfWork.Branding.GetDefaultAsync(cancellationToken);
        var currency = branding?.Currency ?? "EGP";

        await unitOfWork.MonthlyFees.RemoveByYearAsync(command.AcademicYearId, cancellationToken);

        foreach (var item in command.Fees)
        {
            var money = Money.Create(item.Amount, currency);
            var fee = new MonthlyFee(command.AcademicYearId, item.Month, money, item.DueDate);
            await unitOfWork.MonthlyFees.AddAsync(fee, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
