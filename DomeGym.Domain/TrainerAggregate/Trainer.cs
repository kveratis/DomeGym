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
}

public sealed class Trainer : AggregateRoot
{
    /// <summary>
    /// The user id that created this Trainer profile
    /// </summary>
    private readonly Guid _userId;

    private readonly List<Guid> _sessionIds = [];
    private readonly Schedule _schedule;

    public Trainer(
        Guid userId,
        Schedule? schedule = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _userId = userId;
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
}