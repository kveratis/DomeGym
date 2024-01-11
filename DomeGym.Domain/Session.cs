using ErrorOr;

namespace DomeGym.Domain;

public static class SessionErrors
{
    public readonly static Error CannotHaveMoreReservationsThanParticipants = Error.Validation(
        "Session.CannotHaveMoreReservationsThanParticipants",
        "Cannot have more reservations than participants");

    public readonly static Error CannotCancelReservationTooCloseToSession = Error.Validation(
        "Session.CannotCancelReservationTooCloseToSession",
        "Cannot cancel reservation too close to session");

    public readonly static Error ParticipantNotFound = Error.NotFound(
        "Session.ParticipantNotFound",
        "Participant not found");

    public readonly static Error ParticipantsCannotReserveSameSessionTwice = Error.Conflict(
        "Session.ParticipantsCannotReserveSameSessionTwice",
        "Participants cannot reserve twice to the same session");
}

public class Session
{
    private readonly Guid _trainerId;
    private readonly List<Guid> _participantIds = new();
    private readonly int _maxParticipants;

    public Guid Id { get; }

    public DateOnly Date { get; }

    public TimeRange Time { get; }

    public Session(
        DateOnly date,
        TimeRange time,
        int maxParticipants,
        Guid trainerId,
        Guid? id = null)
    {
        Date = date;
        Time = time;
        _maxParticipants = maxParticipants;
        _trainerId = trainerId;
        Id = id ?? Guid.NewGuid();
    }

    public ErrorOr<Success> CancelReservation(Participant participant, IDateTimeProvider dateTimeProvider)
    {
        if (IsTooCloseToSession(dateTimeProvider.UtcNow))
        {
            return SessionErrors.CannotCancelReservationTooCloseToSession;
        }

        if (!_participantIds.Remove(participant.Id))
        {
            return SessionErrors.ParticipantNotFound;
        }

        return Result.Success;
    }

    private bool IsTooCloseToSession(DateTime utcNow)
    {
        const int MinHours = 24;

        return (Date.ToDateTime(Time.Start) - utcNow).TotalHours < MinHours;
    }

    public ErrorOr<Success> ReserveSpot(Participant participant)
    {
        if (_participantIds.Count >= _maxParticipants)
        {
            return SessionErrors.CannotHaveMoreReservationsThanParticipants;
        }

        if (_participantIds.Contains(participant.Id))
        {
            return SessionErrors.ParticipantsCannotReserveSameSessionTwice;
        }

        _participantIds.Add(participant.Id);

        return Result.Success;
    }
}