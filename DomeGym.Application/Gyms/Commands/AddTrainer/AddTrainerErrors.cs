using ErrorOr;

namespace DomeGym.Application.Gyms.Commands.AddTrainer;

public static class AddTrainerErrors
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "AddTrainer.SubscriptionNotFound",
        "Subscription not found");

    public static readonly Error GymNotFound = Error.NotFound(
        "AddTrainer.GymNotFound",
        "Gym not found");

    public static readonly Error TrainerAlreadyInGym = Error.Conflict(
        "AddTrainer.TrainerAlreadyInGym",
        "Trainer already in gym");

    public static readonly Error TrainerNotFound = Error.NotFound(
        "AddTrainer.TrainerNotFound",
        "Trainer not found");
}
