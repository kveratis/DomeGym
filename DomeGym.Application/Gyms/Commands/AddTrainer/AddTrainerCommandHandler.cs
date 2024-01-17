using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using DomeGym.Domain.TrainerAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Commands.AddTrainer;

public sealed class AddTrainerCommandHandler : IRequestHandler<AddTrainerCommand, ErrorOr<Success>>
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "AddTrainerCommandHandler.SubscriptionNotFound",
        "Subscription not found");

    public static readonly Error GymNotFound = Error.NotFound(
        "AddTrainerCommandHandler.GymNotFound",
        "Gym not found");

    public static readonly Error TrainerAlreadyInGym = Error.Conflict(
        "AddTrainerCommandHandler.TrainerAlreadyInGym",
        "Trainer already in gym");

    public static readonly Error TrainerNotFound = Error.NotFound(
        "AddTrainerCommandHandler.TrainerNotFound",
        "Trainer not found");

    private readonly IGymsRepository _gymsRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly ITrainersRepository _trainerRepository;

    public AddTrainerCommandHandler(ITrainersRepository trainersRepository,
        ISubscriptionsRepository subscriptionsRepository, IGymsRepository gymRepository)
    {
        _trainerRepository = trainersRepository;
        _subscriptionsRepository = subscriptionsRepository;
        _gymsRepository = gymRepository;
    }

    public async Task<ErrorOr<Success>> Handle(AddTrainerCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId);

        if (subscription is null)
        {
            return SubscriptionNotFound;
        }

        if (!subscription.HasGym(command.GymId))
        {
            return GymNotFound;
        }

        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return GymNotFound;
        }

        if (gym.HasTrainer(command.TrainerId))
        {
            return TrainerAlreadyInGym;
        }

        var trainer = await _trainerRepository.GetByIdAsync(command.TrainerId);

        if (trainer is null)
        {
            return TrainerNotFound;
        }

        gym.AddTrainer(trainer);

        await _gymsRepository.UpdateAsync(gym);

        return Result.Success;
    }
}