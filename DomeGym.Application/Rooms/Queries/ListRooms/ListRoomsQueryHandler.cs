using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.RoomAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Queries.ListRooms;

public sealed class ListRoomsQueryHandler : IRequestHandler<ListRoomsQuery, ErrorOr<List<Room>>>
{
    public static readonly Error GymNotFound = Error.NotFound(
        "ListRoomsQueryHandler.GymNotFound", 
        "Gym not found");
    
    private readonly IGymsRepository _gymsRepository;
    private readonly IRoomsRepository _roomsRepository;

    public ListRoomsQueryHandler(IRoomsRepository roomsRepository, IGymsRepository gymsRepository)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
    }
    
    public async Task<ErrorOr<List<Room>>> Handle(ListRoomsQuery query, CancellationToken cancellationToken)
    {
        if (!await _gymsRepository.ExistsAsync(query.GymId))
        {
            return GymNotFound;
        }

        return await _roomsRepository.ListByGymIdAsync(query.GymId);
    }
}
