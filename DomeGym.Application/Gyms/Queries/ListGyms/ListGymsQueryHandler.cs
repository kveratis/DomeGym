using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Queries.ListGyms;

public sealed class ListGymsQueryHandler : IRequestHandler<ListGymsQuery, ErrorOr<List<Gym>>>
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "ListGymsQueryHandler.SubscriptionNotFound",
        "Subscription not found");

    private readonly IGymsRepository _gymsRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ListGymsQueryHandler(IGymsRepository gymsRepository, ISubscriptionsRepository subscriptionsRepository)
    {
        _gymsRepository = gymsRepository;
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<List<Gym>>> Handle(ListGymsQuery query, CancellationToken cancellationToken)
    {
        if (!await _subscriptionsRepository.ExistsAsync(query.SubscriptionId))
        {
            return SubscriptionNotFound;
        }

        return await _gymsRepository.ListSubscriptionGymsAsync(query.SubscriptionId);
    }
}