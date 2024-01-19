using ErrorOr;

namespace DomeGym.Application.Subscriptions.Commands.CreateSubscription;

public static class CreateSubscriptionErrors
{
    public static readonly Error AdminNotFound = Error.NotFound(
        "CreateSubscription.AdminNotFound", 
        "Admin not found");

    public static readonly Error AdminAlreadyHasActiveSubscription = Error.Conflict(
        "CreateSubscription.AdminAlreadyHasActiveSubscription", 
        "Admin already has active subscription");
}
