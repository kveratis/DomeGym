using ErrorOr;

namespace DomeGym.Domain;

public static class GymErrors
{
    public static readonly Error RoomAlreadyExistsInGym = Error.Conflict(
        "Gym.RoomAlreadyExistsInGym",
        "Room already exists in gym");

    public static readonly Error CannotHaveMoreRoomsThanSubscriptionAllows = Error.Validation(
        "Gym.CannotHaveMoreRoomsThanSubscriptionAllows",
        "A gym cannot have more rooms than the subscription allows");
}

public class Gym
{
    private readonly Guid _subscriptionId;
    private readonly int _maxRooms;
    private readonly List<Guid> _roomIds = new();

    public Guid Id { get; }

    public Gym(
        int maxRooms,
        Guid subscriptionId,
        Guid? id = null)
    {
        _maxRooms = maxRooms;
        _subscriptionId = subscriptionId;
        Id = id ?? Guid.NewGuid();
    }

    public ErrorOr<Success> AddRoom(Room room)
    {
        if (_roomIds.Contains(room.Id))
        {
            return GymErrors.RoomAlreadyExistsInGym;
        }

        if (_roomIds.Count >= _maxRooms)
        {
            return GymErrors.CannotHaveMoreRoomsThanSubscriptionAllows;
        }

        _roomIds.Add(room.Id);

        return Result.Success;
    }
}