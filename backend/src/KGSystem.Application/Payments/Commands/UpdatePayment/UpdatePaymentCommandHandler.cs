using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Payments.Commands.UpdatePayment;

public sealed class UpdatePaymentCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdatePaymentCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await unitOfWork.Payments.GetByIdAsync(command.Id, cancellationToken);
        if (payment is null)
            throw new NotFoundException(nameof(Domain.Entities.Payment), command.Id);

        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(payment.EnrollmentId, cancellationToken);
        var year = enrollment is not null
            ? await unitOfWork.AcademicYears.GetByIdAsync(enrollment.AcademicYearId, cancellationToken)
            : null;
        if (year is not null && !year.IsActive)
            throw new DomainException("Cannot modify a payment in an archived academic year.");

        var method = Enum.Parse<PaymentMethod>(command.Method);
        payment.UpdatePayment(command.AmountPaid, command.Discount, method, command.Notes, command.ReceivedBy);

        unitOfWork.Payments.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
