using DomeGym.Domain.SessionAggregate;
using ErrorOr;

namespace DomeGym.Domain.TrainerAggregate;

public static class TrainerErrors
{
    public static readonly Error SessionAlreadyExists = Error.Conflict(
        "Trainer.SessionAlreadyExists",
        "Session already exists in trainer's schedule");

    public static readonly Error CannotHaveTwoOrMoreOverlappingSessions = Error.Validation(
        "Trainer.CannotHaveTwoOrMoreOverlappingSessions",
        "A trainer cannot have two or more overlapping sessions");
    
    public static readonly Error SessionNotFound = Error.NotFound(
        "Trainer.SessionNotFound",
        "Session not found");
}

public sealed class Trainer : AggregateRoot
{
    private readonly List<Guid> _sessionIds = [];
    private readonly Schedule _schedule;

    /// <summary>
    /// The user id that created this Trainer profile
    /// </summary>
    public Guid UserId { get; }
    
    public Trainer(
        Guid userId,
        Schedule? schedule = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        _schedule = schedule ?? Schedule.Empty();
    }

    public ErrorOr<Success> AddSessionToSchedule(Session session)
    {
        if (_sessionIds.Contains(session.Id))
        {
            return TrainerErrors.SessionAlreadyExists;
        }

        var bookTimeSlotsResult = _schedule.BookTimeSlot(session.Date, session.Time);

        if (bookTimeSlotsResult.IsError && bookTimeSlotsResult.FirstError.Type == ErrorType.Conflict)
        {
            return TrainerErrors.CannotHaveTwoOrMoreOverlappingSessions;
        }

        _sessionIds.Add(session.Id);

        return Result.Success;
    }
    
    public bool IsTimeSlotFree(DateOnly date, TimeRange time)
    {
        return _schedule.CanBookTimeSlot(date, time);
    }
    
    public ErrorOr<Success> RemoveFromSchedule(Session session)
    {
        if (!_sessionIds.Contains(session.Id))
        {
            return TrainerErrors.SessionNotFound;
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
}