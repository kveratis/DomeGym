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
}

public sealed class Participant : AggregateRoot
{
    private readonly Schedule _schedule = Schedule.Empty();

    /// <summary>
    /// The user id that created this Participant profile
    /// </summary>
    private readonly Guid _userId;

    private readonly List<Guid> _sessionIds = [];

    public Participant(
        Guid userId,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _userId = userId;
    }

    public ErrorOr<Success> AddToSchedule(Session session)
    {
        if (_sessionIds.Contains(session.Id))
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
}