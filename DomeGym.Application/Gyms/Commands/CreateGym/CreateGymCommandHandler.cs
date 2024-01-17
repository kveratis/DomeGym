using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Commands.CreateGym;

public sealed class CreateGymCommandHandler : IRequestHandler<CreateGymCommand, ErrorOr<Gym>>
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "CreateGymCommandHandler.SubscriptionNotFound",
        "Subscription not found");

    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public CreateGymCommandHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<Gym>> Handle(CreateGymCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId);

        if (subscription is null)
        {
            return SubscriptionNotFound;
        }

        var gym = new Gym(
            command.Name,
            subscription.GetMaxRooms(),
            subscription.Id);

        var addGymResult = subscription.AddGym(gym);

        if (addGymResult.IsError)
        {
            return addGymResult.Errors;
        }

        await _subscriptionsRepository.UpdateAsync(subscription);

        return gym;
    }
}