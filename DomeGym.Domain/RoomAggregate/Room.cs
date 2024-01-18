using DomeGym.Domain.SessionAggregate;
using ErrorOr;

namespace DomeGym.Domain.RoomAggregate;

public static class RoomErrors
{
    public static readonly Error SessionAlreadyExists = Error.Conflict(
        "Room.SessionAlreadyExists",
        "Session already exists in room");

    public static readonly Error CannotHaveMoreSessionsThanSubscriptionAllows = Error.Validation(
        "Room.CannotHaveMoreSessionsThanSubscriptionAllows",
        "A room cannot have more scheduled sessions than the subscription allows");

    public static readonly Error CannotHaveTwoOrMoreOverlappingSessions = Error.Validation(
        "Room.CannotHaveTwoOrMoreOverlappingSessions",
        "A room cannot have two or more overlapping sessions");
}

public sealed class Room : AggregateRoot
{
    private readonly Dictionary<DateOnly, List<Guid>> _sessionIdsByDate = [];
    private readonly int _maxDailySessions;
    private readonly Schedule _schedule;

    public string Name { get; } = null!;

    public Guid GymId { get; }
    
    public IReadOnlyList<Guid> SessionIds => _sessionIdsByDate.Values
        .SelectMany(sessionIds => sessionIds)
        .ToList()
        .AsReadOnly();
    
    public Room(
        string name,
        int maxDailySessions,
        Guid gymId,
        Schedule? schedule = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Name = name;
        _maxDailySessions = maxDailySessions;
        GymId = gymId;
        _schedule = schedule ?? Schedule.Empty();
    }

    public ErrorOr<Success> ScheduleSession(Session session)
    {
        if (SessionIds.Any(id => id == session.Id))
        {
            return RoomErrors.SessionAlreadyExists;
        }
        
        if (!_sessionIdsByDate.ContainsKey(session.Date))
        {
            _sessionIdsByDate[session.Date] = new();
        }

        var dailySessions = _sessionIdsByDate[session.Date];
        
        if (dailySessions.Count >= _maxDailySessions)
        {
            return RoomErrors.CannotHaveMoreSessionsThanSubscriptionAllows;
        }

        var addEventResult = _schedule.BookTimeSlot(session.Date, session.Time);

        if (addEventResult.IsError && addEventResult.FirstError.Type == ErrorType.Conflict)
        {
            return RoomErrors.CannotHaveTwoOrMoreOverlappingSessions;
        }

        dailySessions.Add(session.Id);

        return Result.Success;
    }
    
    public bool HasSession(Guid sessionId)
    {
        return SessionIds.Contains(sessionId);
    }
}