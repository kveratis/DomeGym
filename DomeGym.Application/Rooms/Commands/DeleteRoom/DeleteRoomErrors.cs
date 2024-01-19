using ErrorOr;

namespace DomeGym.Application.Rooms.Commands.DeleteRoom;

public static class DeleteRoomErrors
{
    public static readonly Error GymNotFound = Error.NotFound(
        "DeleteRoom.GymNotFound", 
        "Gym not found");

    public static readonly Error RoomNotFound = Error.NotFound(
        "DeleteRoom.RoomNotFound", 
        "Room not found");
}
