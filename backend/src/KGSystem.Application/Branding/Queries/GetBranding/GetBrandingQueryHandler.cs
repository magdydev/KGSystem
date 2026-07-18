using AutoMapper;
using KGSystem.Application.Branding.Dtos;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace KGSystem.Application.Branding.Queries.GetBranding;

public sealed class GetBrandingQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<GetBrandingQueryHandler> logger)
    : IQueryHandler<GetBrandingQuery, BrandingDto?>
{
    public async Task<BrandingDto?> Handle(GetBrandingQuery query, CancellationToken cancellationToken)
    {
        var branding = await unitOfWork.Branding.GetDefaultAsync(cancellationToken);

        if (branding is null)
        {
            logger.LogInformation("No branding settings found — returning defaults");
            return new BrandingDto
            {
                AppName = BrandingDefaults.AppName,
                AppNameAr = BrandingDefaults.AppNameAr,
                LogoUrl = BrandingDefaults.LogoUrl,
                LogoData = null,
                PrimaryColor = BrandingDefaults.PrimaryColor,
                SecondaryColor = BrandingDefaults.SecondaryColor,
                Currency = BrandingDefaults.Currency,
            };
        }

        return mapper.Map<BrandingDto>(branding);
    }
}
