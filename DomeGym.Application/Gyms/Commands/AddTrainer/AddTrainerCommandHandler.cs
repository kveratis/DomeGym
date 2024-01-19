using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using DomeGym.Domain.TrainerAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Commands.AddTrainer;

public sealed class AddTrainerCommandHandler : IRequestHandler<AddTrainerCommand, ErrorOr<Success>>
{
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
            return AddTrainerErrors.SubscriptionNotFound;
        }

        if (!subscription.HasGym(command.GymId))
        {
            return AddTrainerErrors.GymNotFound;
        }

        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return AddTrainerErrors.GymNotFound;
        }

        if (gym.HasTrainer(command.TrainerId))
        {
            return AddTrainerErrors.TrainerAlreadyInGym;
        }

        var trainer = await _trainerRepository.GetByIdAsync(command.TrainerId);

        if (trainer is null)
        {
            return AddTrainerErrors.TrainerNotFound;
        }

        gym.AddTrainer(trainer);

        await _gymsRepository.UpdateAsync(gym);

        return Result.Success;
    }
}
