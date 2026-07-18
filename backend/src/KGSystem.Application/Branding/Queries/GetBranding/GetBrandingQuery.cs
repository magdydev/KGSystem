using KGSystem.Application.Branding.Dtos;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Branding.Queries.GetBranding;

public sealed record GetBrandingQuery : IQuery<BrandingDto?>;
