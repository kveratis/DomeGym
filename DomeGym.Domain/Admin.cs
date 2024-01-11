namespace DomeGym.Domain;

public class Admin
{
    /// <summary>
    /// The user id that created this Admin profile
    /// </summary>
    private readonly Guid _userId;

    /// <summary>
    /// The subscription that this Admin currently has
    /// </summary>
    private readonly Guid _subscriptionId;
}