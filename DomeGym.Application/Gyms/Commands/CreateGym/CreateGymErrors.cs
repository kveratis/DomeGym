using ErrorOr;

namespace DomeGym.Application.Gyms.Commands.CreateGym;

public static class CreateGymErrors
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "CreateGym.SubscriptionNotFound",
        "Subscription not found");
}
