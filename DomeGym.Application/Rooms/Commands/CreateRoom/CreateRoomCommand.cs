using DomeGym.Domain.RoomAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Commands.CreateRoom;

public sealed record CreateRoomCommand(
    Guid GymId,
    string RoomName) : IRequest<ErrorOr<Room>>;
    