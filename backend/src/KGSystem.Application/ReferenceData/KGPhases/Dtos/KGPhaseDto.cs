namespace KGSystem.Application.ReferenceData.KGPhases.Dtos;

public sealed record KGPhaseDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public int SortOrder { get; init; }
}
