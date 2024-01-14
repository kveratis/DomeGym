﻿using DomeGym.Domain.SessionAggregate;
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
    private readonly List<Guid> _sessionIds = [];
    private readonly int _maxDailySessions;
    private readonly Guid _gymId;
    private readonly Schedule _schedule;

    public Room(
        int maxDailySessions,
        Guid gymId,
        Schedule? schedule = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _maxDailySessions = maxDailySessions;
        _gymId = gymId;
        _schedule = schedule ?? Schedule.Empty();
    }

    public ErrorOr<Success> ScheduleSession(Session session)
    {
        if (_sessionIds.Any(id => id == session.Id))
        {
            return RoomErrors.SessionAlreadyExists;
        }

        if (_sessionIds.Count >= _maxDailySessions)
        {
            return RoomErrors.CannotHaveMoreSessionsThanSubscriptionAllows;
        }

        var addEventResult = _schedule.BookTimeSlot(session.Date, session.Time);

        if (addEventResult.IsError && addEventResult.FirstError.Type == ErrorType.Conflict)
        {
            return RoomErrors.CannotHaveTwoOrMoreOverlappingSessions;
        }

        _sessionIds.Add(session.Id);

        return Result.Success;
    }
}