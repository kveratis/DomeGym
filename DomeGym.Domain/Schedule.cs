using ErrorOr;
using System.Security.Cryptography;

namespace DomeGym.Domain;

public static class ScheduleErrors
{
    public static readonly Error Conflict = Error.Conflict(
        "Schedule.Conflict",
        "The requested time conflicts with an existing session");

    public static readonly Error BookingNotFound = Error.Validation(
        "Schedule.BookingNotFound",
        "Booking not found");
}

public sealed class Schedule : Entity
{
    private readonly Dictionary<DateOnly, List<TimeRange>> _calendar;

    public Schedule(
        Dictionary<DateOnly, List<TimeRange>>? calendar = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _calendar = calendar ?? [];
    }

    public static Schedule Empty()
    {
        return new Schedule(id: Guid.NewGuid());
    }

    internal bool CanBookTimeSlot(DateOnly date, TimeRange time)
    {
        if (!_calendar.TryGetValue(date, out var timeSlots))
        {
            return true;
        }

        return !timeSlots.Any(timeSlots => timeSlots.OverlapsWith(time));
    }

    internal ErrorOr<Success> BookTimeSlot(DateOnly date, TimeRange time)
    {
        if (!_calendar.TryGetValue(date, out var timeSlots))
        {
            _calendar[date] = [time];
            return Result.Success;
        }

        if (timeSlots.Any(timeSlot => timeSlot.OverlapsWith(time)))
        {
            return ScheduleErrors.Conflict;
        }

        timeSlots.Add(time);

        return Result.Success;
    }

    internal ErrorOr<Success> RemoveBooking(DateOnly date, TimeRange time)
    {
        if (!_calendar.TryGetValue(date, out var timeSlots) || !timeSlots.Contains(time))
        {
            return ScheduleErrors.BookingNotFound;
        }

        if (!timeSlots.Remove(time))
        {
            return Error.Unexpected();
        }

        return Result.Success;
    }
}