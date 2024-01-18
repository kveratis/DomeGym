using DomeGym.Domain.ParticipantAggregate;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ErrorOr<Success>>
{
    public static readonly Error SessionNotFound = Error.NotFound(
        "CreateReservationCommandHandler.SessionNotFound",
        "Session not found");
    
    public static readonly Error ParticipantAlreadyHasReservation = Error.NotFound(
        "CreateReservationCommandHandler.ParticipantAlreadyHasReservation", 
        "Participant already has reservation");
    
    public static readonly Error ParticipantNotFound = Error.NotFound(
        "CreateReservationCommandHandler.ParticipantNotFound",
        "Participant not found");
    
    public static readonly Error ParticipantNotExpectedToHaveReservation = Error.Unexpected(
        "CreateReservationCommandHandler.ParticipantNotExpectedToHaveReservation", 
        "Participant not expected to have reservation to session");
    
    public static readonly Error ParticipantCalendarNotFreeForSession = Error.Conflict(
        "CreateReservationCommandHandler.ParticipantCalendarNotFreeForSession", 
        "Participant's calendar is not free for the entire session duration");
    
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IParticipantsRepository _participantsRepository;
    
    public CreateReservationCommandHandler(ISessionsRepository sessionsRepository, IParticipantsRepository participantsRepository)
    {
        _sessionsRepository = sessionsRepository;
        _participantsRepository = participantsRepository;
    }
    
    public async Task<ErrorOr<Success>> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.SessionId);

        if (session is null)
        {
            return SessionNotFound;
        }

        if (session.HasReservationForParticipant(command.ParticipantId))
        {
            return ParticipantAlreadyHasReservation;
        }

        var participant = await _participantsRepository.GetByIdAsync(command.ParticipantId);

        if (participant is null)
        {
            return ParticipantNotFound;
        }

        if (participant.HasReservationForSession(session.Id))
        {
            return ParticipantNotExpectedToHaveReservation;
        }

        if (!participant.IsTimeSlotFree(session.Date, session.Time))
        {
            return ParticipantCalendarNotFreeForSession;
        }

        var reserveSpotResult = session.ReserveSpot(participant);

        if (reserveSpotResult.IsError)
        {
            return reserveSpotResult.Errors;
        }

        await _sessionsRepository.UpdateAsync(session);

        return Result.Success;
    }
}