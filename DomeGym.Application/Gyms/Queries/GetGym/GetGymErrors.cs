using ErrorOr;

namespace DomeGym.Application.Gyms.Queries.GetGym;

public static class GetGymErrors
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "GetGym.SubscriptionNotFound",
        "Subscription not found");

    public static readonly Error GymNotFound = Error.NotFound(
        "GetGym.GymNotFound",
        "Gym not found");
}
