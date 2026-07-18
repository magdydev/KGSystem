using AutoMapper;
using KGSystem.Application.Attendance.Dtos;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Attendance.Queries.GetTodayAttendance;

public sealed class GetTodayAttendanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetTodayAttendanceQuery, IReadOnlyList<AttendanceDto>>
{
    public async Task<IReadOnlyList<AttendanceDto>> Handle(GetTodayAttendanceQuery query, CancellationToken cancellationToken)
    {
        var attendances = await unitOfWork.Attendances.GetByDateAsync(DateTime.UtcNow.Date, cancellationToken);

        return mapper.Map<IReadOnlyList<AttendanceDto>>(attendances);
    }
}
