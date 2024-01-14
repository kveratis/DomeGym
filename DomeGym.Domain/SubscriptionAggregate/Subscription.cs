using Ardalis.SmartEnum;
using DomeGym.Domain.GymAggregate;
using ErrorOr;

namespace DomeGym.Domain.SubscriptionAggregate;

public sealed class SubscriptionType : SmartEnum<SubscriptionType>
{
    public static readonly SubscriptionType Free = new(nameof(Free), 0);
    public static readonly SubscriptionType Starter = new(nameof(Starter), 1);
    public static readonly SubscriptionType Pro = new(nameof(Pro), 2);

    public SubscriptionType(string name, int value) : base(name, value)
    {
    }
}

public static class SubscriptionErrors
{
    public static readonly Error GymAlreadyExists = Error.Conflict(
        "Subscription.GymAlreadyExists",
        "Gym already exists");

    public static readonly Error CannotHaveMoreGymsThanSubscriptionAllows = Error.Validation(
        "Subscription.CannotHaveMoreGymsThanSubscriptionAllows",
        "A subscription cannot have more gyms than the subscription allows");
}

public sealed class Subscription : AggregateRoot
{
    private readonly List<Guid> _gymIds = [];
    private readonly SubscriptionType _subscriptionType;
    private readonly int _maxGyms;
    private readonly Guid _adminId;

    public Subscription(
        SubscriptionType subscriptionType,
        Guid adminId,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _subscriptionType = subscriptionType;
        _maxGyms = GetMaxGyms();
        _adminId = adminId;
    }

    public int GetMaxGyms() => _subscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 1,
        nameof(SubscriptionType.Starter) => 1,
        nameof(SubscriptionType.Pro) => 3,
        _ => throw new InvalidOperationException()
    };

    public int GetMaxRooms() => _subscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 1,
        nameof(SubscriptionType.Starter) => 3,
        nameof(SubscriptionType.Pro) => int.MaxValue,
        _ => throw new InvalidOperationException()
    };

    public int GetMaxDailySessions() => _subscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 4,
        nameof(SubscriptionType.Starter) => int.MaxValue,
        nameof(SubscriptionType.Pro) => int.MaxValue,
        _ => throw new InvalidOperationException()
    };

    public ErrorOr<Success> AddGym(Gym gym)
    {
        if (_gymIds.Contains(gym.Id))
        {
            return SubscriptionErrors.GymAlreadyExists;
        }

        if (_gymIds.Count >= _maxGyms)
        {
            return SubscriptionErrors.CannotHaveMoreGymsThanSubscriptionAllows;
        }

        _gymIds.Add(gym.Id);

        return Result.Success;
    }
}