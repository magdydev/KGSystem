using AutoMapper;
using KGSystem.Application.Branding.Dtos;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace KGSystem.Application.Branding.Commands.UpdateBranding;

public sealed class UpdateBrandingCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher,
    IMapper mapper,
    ILogger<UpdateBrandingCommandHandler> logger)
    : ICommandHandler<UpdateBrandingCommand, BrandingDto>
{
    public async Task<BrandingDto> Handle(UpdateBrandingCommand command, CancellationToken cancellationToken)
    {
        var settings = await unitOfWork.Branding.GetDefaultAsync(cancellationToken);

        if (settings is null)
        {
            settings = BrandingSetting.CreateDefault(command.AppName, command.AppNameAr, command.LogoUrl, command.LogoData, command.PrimaryColor, command.SecondaryColor, command.Currency);
            await unitOfWork.Branding.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(command.AppName, command.AppNameAr, command.LogoUrl, command.LogoData, command.PrimaryColor, command.SecondaryColor, command.Currency);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await domainEventDispatcher.DispatchAndClearEvents([settings], cancellationToken);

        logger.LogInformation("Branding settings updated: {AppName}", settings.AppName);

        return mapper.Map<BrandingDto>(settings);
    }
}
