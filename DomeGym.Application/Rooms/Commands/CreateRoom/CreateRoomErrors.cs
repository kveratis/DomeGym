using ErrorOr;

namespace DomeGym.Application.Rooms.Commands.CreateRoom;

public static class CreateRoomErrors
{
    public static readonly Error GymNotFound = Error.NotFound(
        "CreateRoom.GymNotFound", 
        "Gym not found");

    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "CreateRoom.SubscriptionNotFound", 
        "Subscription not found");
}
