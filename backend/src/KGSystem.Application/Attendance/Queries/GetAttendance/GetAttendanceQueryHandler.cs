using AutoMapper;
using KGSystem.Application.Attendance.Dtos;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Attendance.Queries.GetAttendance;

public sealed class GetAttendanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetAttendanceQuery, IReadOnlyList<AttendanceDto>>
{
    public async Task<IReadOnlyList<AttendanceDto>> Handle(GetAttendanceQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Attendance> attendances;

        if (query.Date.HasValue)
        {
            attendances = await unitOfWork.Attendances.GetByDateAsync(query.Date.Value, cancellationToken);
        }
        else
        {
            attendances = await unitOfWork.Attendances.ListAllAsync(cancellationToken);
        }

        var dtos = mapper.Map<List<AttendanceDto>>(attendances);

        if (query.ChildId.HasValue)
            dtos = dtos.Where(d => d.ChildId == query.ChildId.Value).ToList();

        if (!string.IsNullOrWhiteSpace(query.Status))
            dtos = dtos.Where(d => d.Status == query.Status).ToList();

        return dtos.AsReadOnly();
    }
}
