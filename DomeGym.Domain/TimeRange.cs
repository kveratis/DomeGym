using ErrorOr;
using Throw;

namespace DomeGym.Domain;

public static class TimeRangeErrors
{
    public readonly static Error InvalidTimeRange = Error.Validation(
        code: "TimeRange.InvalidTimeRange",
        description: "Time range is not valid");
}

public sealed class TimeRange
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
        if (start.Date != end.Date || start >= end)
        {
            return TimeRangeErrors.InvalidTimeRange;
        }

        return new TimeRange(TimeOnly.FromDateTime(start), TimeOnly.FromDateTime(end));
    }

    public bool OverlapsWith(TimeRange other)
    {
        if (Start >= other.End) return false;
        if (other.Start >= End) return false;

        return true;
    }
}