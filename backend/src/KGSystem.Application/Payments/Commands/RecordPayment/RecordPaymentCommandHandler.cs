using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Payments.Commands.RecordPayment;

public sealed class RecordPaymentCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<RecordPaymentCommand, Guid>
{
    public async Task<Guid> Handle(RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(command.EnrollmentId, cancellationToken);
        if (enrollment is null)
            throw new NotFoundException(nameof(Enrollment), command.EnrollmentId);

        var payment = new Payment(
            command.EnrollmentId,
            command.Month,
            command.Year,
            command.AmountDue,
            command.DueDate);

        var method = Enum.Parse<PaymentMethod>(command.Method);
        payment.RecordPayment(command.AmountPaid, method, command.ReceivedBy, command.PaidDate);

        await unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
