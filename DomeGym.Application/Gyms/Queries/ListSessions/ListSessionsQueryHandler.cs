using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SessionAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Queries.ListSessions;

public sealed class ListSessionsQueryHandler : IRequestHandler<ListSessionsQuery, ErrorOr<List<Session>>>
{
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "ListSessionsQueryHandler.SubscriptionNotFound",
        "Subscription not found");

    public static readonly Error GymNotFound = Error.NotFound(
        "ListSessionsQueryHandler.GymNotFound",
        "Gym not found");

    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsQueryHandler(IGymsRepository gymsRepository, ISubscriptionsRepository subscriptionsRepository, ISessionsRepository sessionsRepository)
    {
        _gymsRepository = gymsRepository;
        _subscriptionsRepository = subscriptionsRepository;
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<List<Session>>> Handle(ListSessionsQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(query.SubscriptionId);

        if (subscription is null)
        {
            return SubscriptionNotFound;
        }

        if (!subscription.HasGym(query.GymId))
        {
            return GymNotFound;
        }

        if (!await _gymsRepository.ExistsAsync(query.GymId))
        {
            return GymNotFound;
        }

        return await _sessionsRepository.ListByGymIdAsync(query.GymId, query.StartDateTime, query.EndDateTime, query.Categories);
    }
}
