using DomeGym.Domain.SessionAggregate;
using DomeGym.Domain.UnitTests.TestConstants;
using DomeGym.Domain.UnitTests.TestUtils.Participants;
using DomeGym.Domain.UnitTests.TestUtils.Services;
using DomeGym.Domain.UnitTests.TestUtils.Sessions;
using FluentAssertions;
using Xunit;

namespace DomeGym.Domain.UnitTests.SessionAggregate;

public sealed class SessionTests
{
    [Fact]
    public void ReserveSpot_WhenNoMoreRoom_ShouldFail()
    {
        // Arrange
        var session = SessionFactory.CreateSession(maxParticipants: 1);
        var participant1 = ParticipantFactory.CreateParticipant(id: Guid.NewGuid(), userId: Guid.NewGuid());
        var participant2 = ParticipantFactory.CreateParticipant(id: Guid.NewGuid(), userId: Guid.NewGuid());

        // Act
        var reserveParticipant1Result = session.ReserveSpot(participant1);
        var reserveParticipant2Result = session.ReserveSpot(participant2);

        // Assert
        reserveParticipant1Result.IsError.Should().BeFalse();

        reserveParticipant2Result.IsError.Should().BeTrue();
        reserveParticipant2Result.FirstError.Should().Be(SessionErrors.CannotHaveMoreReservationsThanParticipants);
    }

    [Fact]
    public void CancelReservation_WhenCancellationIsTooCloseToSession_ShouldFail()
    {
        // Arrange
        var session = SessionFactory.CreateSession(
            date: Constants.Session.Date,
            time: Constants.Session.Time);

        var participant = ParticipantFactory.CreateParticipant();

        var cancellationDateTime = Constants.Session.Date.ToDateTime(TimeOnly.MinValue);

        // Act
        var reserveSpotResult = session.ReserveSpot(participant);
        var cancelReservationResult = session.CancelReservation(
            participant,
            new TestDateTimeProvider(fixedDateTime: cancellationDateTime));

        // Assert
        reserveSpotResult.IsError.Should().BeFalse();

        cancelReservationResult.IsError.Should().BeTrue();
        cancelReservationResult.FirstError.Should().Be(SessionErrors.CannotCancelReservationTooCloseToSession);
    }
}