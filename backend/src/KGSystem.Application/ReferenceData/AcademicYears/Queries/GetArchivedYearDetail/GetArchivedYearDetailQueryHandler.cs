using AutoMapper;
using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Enrollments.Dtos;
using KGSystem.Application.Payments.Dtos;
using KGSystem.Application.ReferenceData.AcademicYears.Dtos;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.AcademicYears.Queries.GetArchivedYearDetail;

public sealed class GetArchivedYearDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetArchivedYearDetailQuery, ArchivedYearDetailDto>
{
    public async Task<ArchivedYearDetailDto> Handle(GetArchivedYearDetailQuery query, CancellationToken cancellationToken)
    {
        var year = await unitOfWork.AcademicYears.GetByIdAsync(query.Id, cancellationToken);
        if (year is null)
            throw new NotFoundException(nameof(AcademicYear), query.Id);

        var enrollments = await unitOfWork.Enrollments.GetByAcademicYearAsync(query.Id, cancellationToken);
        var payments = await unitOfWork.Payments.GetByAcademicYearAsync(query.Id, cancellationToken);

        return new ArchivedYearDetailDto
        {
            Year = mapper.Map<AcademicYearDto>(year),
            Enrollments = mapper.Map<IReadOnlyList<EnrollmentDto>>(enrollments),
            Payments = mapper.Map<IReadOnlyList<PaymentDto>>(payments),
        };
    }
}
