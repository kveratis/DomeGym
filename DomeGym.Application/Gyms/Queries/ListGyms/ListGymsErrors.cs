using ErrorOr;

namespace DomeGym.Application.Gyms.Queries.ListGyms;

public static class ListGymsErrors
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "ListGyms.SubscriptionNotFound",
        "Subscription not found");
}
