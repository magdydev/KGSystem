using KGSystem.Application.Enrollments.Dtos;
using KGSystem.Application.Payments.Dtos;

namespace KGSystem.Application.ReferenceData.AcademicYears.Dtos;

public sealed record ArchivedYearDetailDto
{
    public AcademicYearDto Year { get; init; } = null!;
    public IReadOnlyList<EnrollmentDto> Enrollments { get; init; } = [];
    public IReadOnlyList<PaymentDto> Payments { get; init; } = [];
}
