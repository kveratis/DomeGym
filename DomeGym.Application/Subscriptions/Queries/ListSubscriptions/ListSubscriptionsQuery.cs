using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Subscriptions.Queries.ListSubscriptions;

public sealed record ListSubscriptionsQuery() : IRequest<ErrorOr<List<Subscription>>>;
