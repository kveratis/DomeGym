using DomeGym.Domain.GymAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Commands.CreateGym;

public sealed record CreateGymCommand(string Name, Guid SubscriptionId) : IRequest<ErrorOr<Gym>>;
