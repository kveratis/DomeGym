using DomeGym.Domain.ParticipantAggregate;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ErrorOr<Success>>
{
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
            return CreateReservationErrors.SessionNotFound;
        }

        if (session.HasReservationForParticipant(command.ParticipantId))
        {
            return CreateReservationErrors.ParticipantAlreadyHasReservation;
        }

        var participant = await _participantsRepository.GetByIdAsync(command.ParticipantId);

        if (participant is null)
        {
            return CreateReservationErrors.ParticipantNotFound;
        }

        if (participant.HasReservationForSession(session.Id))
        {
            return CreateReservationErrors.ParticipantNotExpectedToHaveReservation;
        }

        if (!participant.IsTimeSlotFree(session.Date, session.Time))
        {
            return CreateReservationErrors.ParticipantCalendarNotFreeForSession;
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
