using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.RoomAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Commands.DeleteRoom;

public sealed class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, ErrorOr<Deleted>>
{
    public static readonly Error GymNotFound = Error.NotFound(
        "DeleteRoomCommandHandler.GymNotFound", 
        "Gym not found");
    
    public static readonly Error RoomNotFound = Error.NotFound(
        "DeleteRoomCommandHandler.RoomNotFound", 
        "Room not found");
    
    private readonly IGymsRepository _gymsRepository;
    private readonly IRoomsRepository _roomsRepository;

    public DeleteRoomCommandHandler(IRoomsRepository roomsRepository, IGymsRepository gymsRepository)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
    }
    
    public async Task<ErrorOr<Deleted>> Handle(DeleteRoomCommand command, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return GymNotFound;
        }

        if (!gym.HasRoom(command.RoomId))
        {
            return RoomNotFound;
        }

        var room = await _roomsRepository.GetByIdAsync(command.RoomId);

        if (room is null)
        {
            return RoomNotFound;
        }

        var removeGymResult = gym.RemoveRoom(room);

        if (removeGymResult.IsError)
        {
            return removeGymResult.Errors;
        }

        await _gymsRepository.UpdateAsync(gym);

        return Result.Deleted;
    }
}