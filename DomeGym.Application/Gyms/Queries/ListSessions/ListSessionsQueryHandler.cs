using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.SessionAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Queries.ListSessions;

public sealed class ListSessionsQueryHandler : IRequestHandler<ListSessionsQuery, ErrorOr<List<Session>>>
{
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
            return ListSessionsErrors.SubscriptionNotFound;
        }

        if (!subscription.HasGym(query.GymId))
        {
            return ListSessionsErrors.GymNotFound;
        }

        if (!await _gymsRepository.ExistsAsync(query.GymId))
        {
            return ListSessionsErrors.GymNotFound;
        }

        return await _sessionsRepository.ListByGymIdAsync(query.GymId, query.StartDateTime, query.EndDateTime, query.Categories);
    }
}
