using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.RoomAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Queries.GetRoom;

public sealed class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, ErrorOr<Room>>
{
    public static readonly Error GymNotFound = Error.NotFound(
        "GetRoomQueryHandler.GymNotFound", 
        "Gym not found");
    
    public static readonly Error RoomNotFound = Error.NotFound(
        "GetRoomQueryHandler.RoomNotFound", 
        "Room not found");
    
    private readonly IGymsRepository _gymsRepository;
    private readonly IRoomsRepository _roomsRepository;

    public GetRoomQueryHandler(IRoomsRepository roomsRepository, IGymsRepository gymsRepository)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
    }
    
    public async Task<ErrorOr<Room>> Handle(GetRoomQuery query, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(query.GymId);

        if (gym is null)
        {
            return GymNotFound;
        }

        if (!gym.HasRoom(query.RoomId))
        {
            return RoomNotFound;
        }

        if (await _roomsRepository.GetByIdAsync(query.RoomId) is not Room room)
        {
            return RoomNotFound;
        }

        return room;
    }
}
