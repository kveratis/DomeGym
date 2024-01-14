using DomeGym.Domain.RoomAggregate;
using ErrorOr;

namespace DomeGym.Domain.GymAggregate;

public static class GymErrors
{
    public static readonly Error RoomAlreadyExistsInGym = Error.Conflict(
        "Gym.RoomAlreadyExistsInGym",
        "Room already exists in gym");

    public static readonly Error CannotHaveMoreRoomsThanSubscriptionAllows = Error.Validation(
        "Gym.CannotHaveMoreRoomsThanSubscriptionAllows",
        "A gym cannot have more rooms than the subscription allows");
}

public sealed class Gym : AggregateRoot
{
    private readonly Guid _subscriptionId;
    private readonly int _maxRooms;
    private readonly List<Guid> _roomIds = [];

    public Gym(
        int maxRooms,
        Guid subscriptionId,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _maxRooms = maxRooms;
        _subscriptionId = subscriptionId;
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