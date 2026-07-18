using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Payments.Commands.RecordPayment;

public sealed class RecordPaymentCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<RecordPaymentCommand, int>
{
    public async Task<int> Handle(RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(command.EnrollmentId, cancellationToken);
        if (enrollment is null)
            throw new NotFoundException(nameof(Enrollment), command.EnrollmentId);

        var year = await unitOfWork.AcademicYears.GetByIdAsync(enrollment.AcademicYearId, cancellationToken);
        if (year is not null && !year.IsActive)
            throw new DomainException("Cannot record a payment for an enrollment in an archived academic year.");

        var payment = new Payment(
            command.EnrollmentId,
            command.Month,
            command.Year,
            command.AmountDue,
            command.DueDate);

        var method = Enum.Parse<PaymentMethod>(command.Method);
        payment.RecordPayment(command.AmountPaid, method, command.ReceivedBy, command.PaidDate, command.Discount);

        await unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
