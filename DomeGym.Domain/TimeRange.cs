using ErrorOr;
using Throw;

namespace DomeGym.Domain;

public static class TimeRangeErrors
{
    public static readonly Error StartAndEndDateMustBeOnSameDay = Error.Validation(
        "TimeRange.StartAndEndDateMustBeOnSameDay",
        "Start and end date times must be on the same day.");

    public static readonly Error EndTimeMustBeGreaterThanTheStartTime = Error.Validation(
        "TimeRange.EndTimeMustBeGreaterThanTheStartTime",
        "End time must be greater than the start time");
}

public sealed class TimeRange : ValueObject
{
    public TimeOnly Start { get; }

    public TimeOnly End { get; }

    public TimeRange(TimeOnly start, TimeOnly end)
    {
        Start = start.Throw().IfGreaterThanOrEqualTo(end);
        End = end;
    }

    public static ErrorOr<TimeRange> FromDateTimes(DateTime start, DateTime end)
    {
        if (start.Date != end.Date)
        {
            return TimeRangeErrors.StartAndEndDateMustBeOnSameDay;
        }

        if (start >= end)
        {
            return TimeRangeErrors.EndTimeMustBeGreaterThanTheStartTime;
        }

        return new TimeRange(TimeOnly.FromDateTime(start), TimeOnly.FromDateTime(end));
    }

    public bool OverlapsWith(TimeRange other)
    {
        if (Start >= other.End) return false;
        if (other.Start >= End) return false;

        return true;
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}