using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.KGPhases.Commands.CreateKGPhase;

public sealed class CreateKGPhaseCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateKGPhaseCommand, Guid>
{
    public async Task<Guid> Handle(CreateKGPhaseCommand command, CancellationToken cancellationToken)
    {
        var phase = new KGPhase(command.Code, command.NameAr, command.NameEn, command.SortOrder, command.DescriptionAr, command.DescriptionEn);

        await unitOfWork.KGPhases.AddAsync(phase, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return phase.Id;
    }
}
