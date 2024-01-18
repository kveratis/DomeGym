using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;

namespace DomeGym.Domain.AdminAggregate;

public static class AdminErrors
{
    public static readonly Error AlreadyHasActiveSubscription = Error.Conflict(
        "Admin.AlreadyHasActiveSubscription",
        "Admin already has an active subscription");
}

public sealed class Admin : AggregateRoot
{
    /// <summary>
    /// The user id that created this Admin profile
    /// </summary>
    public Guid UserId { get; }
    
    /// <summary>
    /// The subscription that this Admin currently has
    /// </summary>
    public Guid? SubscriptionId { get; private set; }

    public Admin(
        Guid userId,
        Guid? subscriptionId = null,
        Guid? id = null) 
        : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
    }
    
    public ErrorOr<Success> SetSubscription(Subscription subscription)
    {
        if (SubscriptionId.HasValue)
        {
            return AdminErrors.AlreadyHasActiveSubscription;
        }

        SubscriptionId = subscription.Id;

        return Result.Success;
    }
}
