using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Queries.GetGym;

public sealed class GetGymQueryHandler : IRequestHandler<GetGymQuery, ErrorOr<Gym>>
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "GetGymQueryHandler.SubscriptionNotFound",
        "Subscription not found");

    public static readonly Error GymNotFound = Error.NotFound(
        "GetGymQueryHandler.GymNotFound",
        "Gym not found");

    private readonly IGymsRepository _gymsRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public GetGymQueryHandler(IGymsRepository gymsRepository, ISubscriptionsRepository subscriptionsRepository)
    {
        _gymsRepository = gymsRepository;
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<Gym>> Handle(GetGymQuery request, CancellationToken cancellationToken)
    {
        if (await _subscriptionsRepository.ExistsAsync(request.SubscriptionId))
        {
            return SubscriptionNotFound;
        }

        if (await _gymsRepository.GetByIdAsync(request.GymId) is not Gym gym)
        {
            return GymNotFound;
        }

        return gym;
    }
}