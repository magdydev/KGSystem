using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.KGPhases.Commands.UpdateKGPhase;

public sealed class UpdateKGPhaseCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateKGPhaseCommand, Unit>
{
    public async Task<Unit> Handle(UpdateKGPhaseCommand command, CancellationToken cancellationToken)
    {
        var phase = await unitOfWork.KGPhases.GetByIdAsync(command.Id, cancellationToken);
        if (phase is null)
            throw new NotFoundException(nameof(Domain.Entities.KGPhase), command.Id);

        phase.Update(command.NameAr, command.NameEn, command.SortOrder, command.DescriptionAr, command.DescriptionEn);

        unitOfWork.KGPhases.Update(phase);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
