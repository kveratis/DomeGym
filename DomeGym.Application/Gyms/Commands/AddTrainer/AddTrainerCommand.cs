using ErrorOr;
using MediatR;

namespace DomeGym.Application.Gyms.Commands.AddTrainer;

public sealed record AddTrainerCommand(Guid SubscriptionId, Guid GymId, Guid TrainerId)
    : IRequest<ErrorOr<Success>>;