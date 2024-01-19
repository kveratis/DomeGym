using ErrorOr;

namespace DomeGym.Application.Gyms.Queries.ListSessions;

public static class ListSessionsErrors
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "ListSessions.SubscriptionNotFound",
        "Subscription not found");

    public static readonly Error GymNotFound = Error.NotFound(
        "ListSessions.GymNotFound",
        "Gym not found");
}
