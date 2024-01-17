using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Queries.ListSessions;

public record ListSessionsQuery(
    Guid SubscriptionId,
    Guid GymId,
    DateTime? StartDateTime = null,
    DateTime? EndDateTime = null,
    List<SessionCategory>? Categories = null) : IRequest<ErrorOr<List<Session>>>;
