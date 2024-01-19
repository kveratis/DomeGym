using DomeGym.Domain;
using DomeGym.Domain.ParticipantAggregate;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Participants.Commands.CancelReservation;

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, ErrorOr<Deleted>>
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IParticipantsRepository _participantsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public CancelReservationCommandHandler(IParticipantsRepository participantsRepository, ISessionsRepository sessionsRepository, IDateTimeProvider dateTimeProvider)
    {
        _participantsRepository = participantsRepository;
        _sessionsRepository = sessionsRepository;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<ErrorOr<Deleted>> Handle(CancelReservationCommand command, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.SessionId);

        if (session is null)
        {
            return CancelReservationErrors.SessionNotFound;
        }

        if (!session.HasReservationForParticipant(command.ParticipantId))
        {
            return CancelReservationErrors.UserDoesntHaveReservation;
        }

        var participant = await _participantsRepository.GetByIdAsync(command.ParticipantId);

        if (participant is null)
        {
            return CancelReservationErrors.ParticipantNotFound;
        }

        if (!participant.HasReservationForSession(session.Id))
        {
            return CancelReservationErrors.ParticipantExpectedToHaveReservation;
        }

        var cancelReservationResult = session.CancelReservation(participant, _dateTimeProvider);

        if (cancelReservationResult.IsError)
        {
            return cancelReservationResult.Errors;
        }

        await _sessionsRepository.UpdateAsync(session);

        return Result.Deleted;
    }
}
