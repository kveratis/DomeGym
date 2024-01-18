using ErrorOr;
using MediatR;

namespace DomeGym.Application.Reservations.Commands.CreateReservation;

public sealed record CreateReservationCommand(
    Guid SessionId,
    Guid ParticipantId) : IRequest<ErrorOr<Success>>;
    