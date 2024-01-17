using DomeGym.Domain.RoomAggregate;
using DomeGym.Domain.TrainerAggregate;
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

    public static readonly Error TrainerAlreadyAssignedToGym = Error.Conflict(
        "Gym.TrainerAlreadyAssignedToGym",
        "Trainer already assigned to gym");
}

public sealed class Gym : AggregateRoot
{
    private readonly int _maxRooms;
    private readonly List<Guid> _roomIds = [];
    private readonly List<Guid> _trainerIds = [];

    public string Name { get; }
    public Guid SubscriptionId { get; }

    public Gym(
        string name,
        int maxRooms,
        Guid subscriptionId,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Name = name;
        _maxRooms = maxRooms;
        SubscriptionId = subscriptionId;
    }

    public ErrorOr<Success> AddRoom(Room room)
    {
        if (HasRoom(room.Id))
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

    public bool HasRoom(Guid roomId)
    {
        return _roomIds.Contains(roomId);
    }

    public ErrorOr<Success> AddTrainer(Trainer trainer)
    {
        if (HasTrainer(trainer.Id))
        {
            return GymErrors.TrainerAlreadyAssignedToGym;
        }

        _trainerIds.Add(trainer.Id);

        return Result.Success;
    }

    public bool HasTrainer(Guid trainerId)
    {
        return _trainerIds.Contains(trainerId);
    }
}
