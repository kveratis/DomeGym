using System.Net.Http.Headers;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;

namespace DomeGym.Domain.ParticipantAggregate;

public static class ParticipantErrors
{
    public static readonly Error SessionAlreadyExists = Error.Conflict(
        "Participant.SessionAlreadyExists",
        "Session already exists in participant's schedule");

    public static readonly Error CannotHaveTwoOrMoreOverlappingSessions = Error.Validation(
        "Participant.CannotHaveTwoOrMoreOverlappingSessions",
        "A participant cannot have two or more overlapping sessions");
    
    public static readonly Error SessionNotFound = Error.NotFound(
        "Participant.SessionNotFound",
        "Session not found");
}

public sealed class Participant : AggregateRoot
{
    private readonly Schedule _schedule;
    private readonly List<Guid> _sessionIds = [];
    
    /// <summary>
    /// The user id that created this Participant profile
    /// </summary>
    public Guid UserId { get; }
    
    public IReadOnlyList<Guid> SessionIds => _sessionIds;
    
    public Participant(
        Guid userId,
        Schedule? schedule = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        _schedule = schedule ?? Schedule.Empty();
    }

    public ErrorOr<Success> AddToSchedule(Session session)
    {
        if (HasReservationForSession(session.Id))
        {
            return ParticipantErrors.SessionAlreadyExists;
        }

        var bookTimeSlotResult = _schedule.BookTimeSlot(
            session.Date,
            session.Time);

        if (bookTimeSlotResult.IsError)
        {
            return bookTimeSlotResult.FirstError.Type == ErrorType.Conflict
                ? ParticipantErrors.CannotHaveTwoOrMoreOverlappingSessions
                : bookTimeSlotResult.Errors;
        }

        _sessionIds.Add(session.Id);

        return Result.Success;
    }
    
    public bool HasReservationForSession(Guid sessionId)
    {
        return _sessionIds.Contains(sessionId);
    }

    public ErrorOr<Success> RemoveFromSchedule(Session session)
    {
        if (!HasReservationForSession(session.Id))
        {
            return ParticipantErrors.SessionNotFound;
        }

        var removeBookingResult = _schedule.RemoveBooking(
            session.Date,
            session.Time);

        if (removeBookingResult.IsError)
        {
            return removeBookingResult.Errors;
        }

        _sessionIds.Remove(session.Id);

        return Result.Success;
    }
    
    public bool IsTimeSlotFree(DateOnly date, TimeRange time)
    {
        return _schedule.CanBookTimeSlot(date, time);
    }
}
