using ErrorOr;

namespace DomeGym.Application.Rooms.Queries.ListRooms;

public static class ListRoomsErrors
{
    public static readonly Error GymNotFound = Error.NotFound(
        "ListRooms.GymNotFound", 
        "Gym not found");
}
