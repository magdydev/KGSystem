using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Children.Commands.UpdateChild;

public sealed class UpdateChildCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateChildCommand, Unit>
{
    public async Task<Unit> Handle(UpdateChildCommand command, CancellationToken cancellationToken)
    {
        var child = await unitOfWork.Children.GetByIdAsync(command.Id, cancellationToken);
        if (child is null)
            throw new NotFoundException(nameof(Domain.Entities.Child), command.Id);

        var gender = Enum.Parse<Gender>(command.Gender);

        child.UpdatePersonalInfo(
            command.FirstNameAr,
            command.FirstNameEn,
            command.LastNameAr,
            command.LastNameEn,
            command.DateOfBirth,
            gender,
            command.Nationality,
            command.Address);

        child.UpdateGuardianInfo(
            command.GuardianNameAr,
            command.GuardianNameEn,
            command.GuardianPhone,
            command.GuardianEmail);

        unitOfWork.Children.Update(child);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
