namespace DomeGym.Domain.AdminAggregate;

public sealed class Admin : AggregateRoot
{
    /// <summary>
    /// The user id that created this Admin profile
    /// </summary>
    private readonly Guid _userId;

    /// <summary>
    /// The subscription that this Admin currently has
    /// </summary>
    private readonly Guid _subscriptionId;

    public Admin(
        Guid userId,
        Guid subscriptionId,
        Guid? id = null) 
        : base(id ?? Guid.NewGuid())
    {
        _userId = userId;
        _subscriptionId = subscriptionId;
    }
}