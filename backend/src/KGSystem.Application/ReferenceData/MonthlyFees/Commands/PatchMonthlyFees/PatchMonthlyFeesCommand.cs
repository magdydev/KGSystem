using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Commands.PatchMonthlyFees;

public sealed record MonthlyFeeItem(
    int Month,
    decimal Amount,
    DateTime? DueDate);

public sealed record PatchMonthlyFeesCommand(
    int AcademicYearId,
    IReadOnlyList<MonthlyFeeItem> Fees) : ICommand<Unit>;
