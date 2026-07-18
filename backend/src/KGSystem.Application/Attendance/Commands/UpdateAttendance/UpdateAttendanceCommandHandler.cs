using KGSystem.Application.Common;
using KGSystem.Application.Common.Exceptions;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Attendance.Commands.UpdateAttendance;

public sealed class UpdateAttendanceCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateAttendanceCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAttendanceCommand command, CancellationToken cancellationToken)
    {
        var attendance = await unitOfWork.Attendances.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Attendance), command.Id);

        var status = Enum.Parse<AttendanceStatus>(command.Status);
        attendance.UpdateStatus(status, command.Notes);
        unitOfWork.Attendances.Update(attendance);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Unit();
    }
}
