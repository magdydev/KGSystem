using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Attendance.Commands.RecordAttendance;

public sealed class RecordAttendanceCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<RecordAttendanceCommand, int>
{
    public async Task<int> Handle(RecordAttendanceCommand command, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Attendances.GetByChildAndDateAsync(command.ChildId, command.Date, cancellationToken);

        if (existing is not null)
        {
            var status = Enum.Parse<AttendanceStatus>(command.Status);
            existing.UpdateStatus(status, command.Notes);
            unitOfWork.Attendances.Update(existing);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var statusEnum = Enum.Parse<AttendanceStatus>(command.Status);
        var attendance = new Domain.Entities.Attendance(command.ChildId, command.Date, statusEnum, command.Notes);

        await unitOfWork.Attendances.AddAsync(attendance, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return attendance.Id;
    }
}
