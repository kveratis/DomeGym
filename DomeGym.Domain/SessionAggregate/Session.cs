using DomeGym.Domain.ParticipantAggregate;
using ErrorOr;

namespace DomeGym.Domain.SessionAggregate;

public static class SessionErrors
{
    public static readonly Error CannotHaveMoreReservationsThanParticipants = Error.Validation(
        "Session.CannotHaveMoreReservationsThanParticipants",
        "Cannot have more reservations than participants");

    public static readonly Error CannotCancelReservationTooCloseToSession = Error.Validation(
        "Session.CannotCancelReservationTooCloseToSession",
        "Cannot cancel reservation too close to session");

    public static readonly Error ParticipantNotFound = Error.NotFound(
        "Session.ParticipantNotFound",
        "Participant not found");

    public static readonly Error ParticipantsCannotReserveSameSessionTwice = Error.Conflict(
        "Session.ParticipantsCannotReserveSameSessionTwice",
        "Participants cannot reserve twice to the same session");

    public static readonly Error ReservationNotFound = Error.NotFound(
        "Session.ReservationNotFound",
        "Reservation not found");
}

public sealed class Session : AggregateRoot
{
    private readonly Guid _trainerId;
    private readonly List<Reservation> _reservations = [];
    private readonly int _maxParticipants;

    public DateOnly Date { get; }

    public TimeRange Time { get; }

    public Session(
        DateOnly date,
        TimeRange time,
        int maxParticipants,
        Guid trainerId,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Date = date;
        Time = time;
        _maxParticipants = maxParticipants;
        _trainerId = trainerId;
    }

    public ErrorOr<Success> CancelReservation(Participant participant, IDateTimeProvider dateTimeProvider)
    {
        if (IsTooCloseToSession(dateTimeProvider.UtcNow))
        {
            return SessionErrors.CannotCancelReservationTooCloseToSession;
        }

        var reservation = _reservations.Find(reservation => reservation.ParticipantId == participant.Id);
        if (reservation is null)
        {
            return SessionErrors.ReservationNotFound;
        }

        _reservations.Remove(reservation);

        return Result.Success;
    }

    private bool IsTooCloseToSession(DateTime utcNow)
    {
        const int MinHours = 24;

        return (Date.ToDateTime(Time.Start) - utcNow).TotalHours < MinHours;
    }

    public ErrorOr<Success> ReserveSpot(Participant participant)
    {
        if (_reservations.Count >= _maxParticipants)
        {
            return SessionErrors.CannotHaveMoreReservationsThanParticipants;
        }

        if (_reservations.Any(reservation => reservation.ParticipantId == participant.Id))
        {
            return SessionErrors.ParticipantsCannotReserveSameSessionTwice;
        }

        var reservation = new Reservation(participant.Id);

        _reservations.Add(reservation);

        return Result.Success;
    }
}