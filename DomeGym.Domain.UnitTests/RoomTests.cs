using DomeGym.Domain.UnitTests.TestConstants;
using DomeGym.Domain.UnitTests.TestUtils.Common;
using DomeGym.Domain.UnitTests.TestUtils.Rooms;
using DomeGym.Domain.UnitTests.TestUtils.Sessions;
using FluentAssertions;
using Xunit;

namespace DomeGym.Domain.UnitTests;

public sealed class RoomTests
{
    [Fact]
    public void ScheduleSession_WhenMoreThanSubscriptionAllows_ShouldFail()
    {
        // Arrange
        var room = RoomFactory.CreateRoom(maxDailySessions: 1);

        var session1 = SessionFactory.CreateSession(id: Guid.NewGuid());
        var session2 = SessionFactory.CreateSession(id: Guid.NewGuid());

        // Act
        var scheduledSession1Result = room.ScheduleSession(session1);
        var scheduledSession2Result = room.ScheduleSession(session2);

        // Assert
        scheduledSession1Result.IsError.Should().BeFalse();

        scheduledSession2Result.IsError.Should().BeTrue();
        scheduledSession2Result.FirstError.Should().Be(RoomErrors.CannotHaveMoreSessionsThanSubscriptionAllows);
    }

    [Theory]
    [InlineData(1, 3, 1, 3)] // exact overlap
    [InlineData(1, 3, 2, 3)] // second session inside first session
    [InlineData(1, 3, 2, 4)] // second session ends after session, but overlaps
    [InlineData(1, 3, 0, 2)] // second session starts before first session, but overlaps
    public void ScheduleSession_WhenSessionOverlapsWithAnotherSession_ShouldFail(
        int startHourSession1,
        int endHourSession1,
        int startHourSession2,
        int endHourSession2)
    {
        // Arrange
        var room = RoomFactory.CreateRoom(maxDailySessions: 2);

        var session1 = SessionFactory.CreateSession(
            date: Constants.Session.Date,
            time: TimeRangeFactory.CreateFromHours(startHourSession1, endHourSession1),
            id: Guid.NewGuid());

        var session2 = SessionFactory.CreateSession(
            date: Constants.Session.Date,
            time: TimeRangeFactory.CreateFromHours(startHourSession2, endHourSession2),
            id: Guid.NewGuid());

        // Act

        var scheduledSession1Result = room.ScheduleSession(session1);
        var scheduledSession2Result = room.ScheduleSession(session2);

        // Assert
        scheduledSession1Result.IsError.Should().BeFalse();

        scheduledSession2Result.IsError.Should().BeTrue();
        scheduledSession2Result.FirstError.Should().Be(RoomErrors.CannotHaveTwoOrMoreOverlappingSessions);
    }
}