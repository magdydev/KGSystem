using KGSystem.Domain.Common;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class KGPhase : BaseEntity, IAggregateRoot
{
    public string Code { get; private set; }
    public string NameAr { get; private set; }
    public string NameEn { get; private set; }
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<Enrollment> _enrollments = [];
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private KGPhase()
    {
        Code = null!;
        NameAr = null!;
        NameEn = null!;
    }

    public KGPhase(string code, string nameAr, string nameEn, int sortOrder, string? descriptionAr = null, string? descriptionEn = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Phase code is required.");
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic phase name is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English phase name is required.");
        if (sortOrder < 0)
            throw new DomainException("Sort order cannot be negative.");

        Code = code.Trim().ToUpperInvariant();
        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        SortOrder = sortOrder;
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();
    }

    public void Update(string nameAr, string nameEn, int sortOrder, string? descriptionAr = null, string? descriptionEn = null)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic phase name is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English phase name is required.");
        if (sortOrder < 0)
            throw new DomainException("Sort order cannot be negative.");

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        SortOrder = sortOrder;
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();
    }
}
