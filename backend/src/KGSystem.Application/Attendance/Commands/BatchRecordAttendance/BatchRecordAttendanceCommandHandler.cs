using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Attendance.Commands.BatchRecordAttendance;

public sealed class BatchRecordAttendanceCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<BatchRecordAttendanceCommand, Unit>
{
    public async Task<Unit> Handle(BatchRecordAttendanceCommand command, CancellationToken cancellationToken)
    {
        var childIds = command.Records.Select(r => r.ChildId).ToList();

        await unitOfWork.Attendances.RemoveByChildIdsAndDateAsync(childIds, command.Date, cancellationToken);

        foreach (var record in command.Records)
        {
            var status = Enum.Parse<AttendanceStatus>(record.Status);
            var attendance = new Domain.Entities.Attendance(record.ChildId, command.Date, status, record.Notes);
            await unitOfWork.Attendances.AddAsync(attendance, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}
