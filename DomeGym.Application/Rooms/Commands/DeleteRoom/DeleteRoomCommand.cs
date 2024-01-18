using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Commands.DeleteRoom;

public sealed record DeleteRoomCommand(Guid GymId, Guid RoomId)
    : IRequest<ErrorOr<Deleted>>;
    