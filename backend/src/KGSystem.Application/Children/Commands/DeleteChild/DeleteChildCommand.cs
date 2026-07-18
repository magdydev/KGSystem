using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Children.Commands.DeleteChild;

public sealed record DeleteChildCommand(Guid Id) : ICommand<Unit>;
