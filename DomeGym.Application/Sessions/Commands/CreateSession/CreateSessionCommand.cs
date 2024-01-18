using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Sessions.Commands.CreateSession;

public sealed record CreateSessionCommand(
    Guid RoomId,
    string Name,
    string Description,
    int MaxParticipants,
    DateTime StartDateTime,
    DateTime EndDateTime,
    Guid TrainerId,
    List<SessionCategory> Categories) : IRequest<ErrorOr<Session>>;
    