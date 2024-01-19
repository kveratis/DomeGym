using ErrorOr;

namespace DomeGym.Application.Participants.Commands.CancelReservation;

public static class CancelReservationErrors
{
    public static readonly Error SessionNotFound = Error.NotFound(
        "CancelReservation.SessionNotFound",
        "Session not found");

    public static readonly Error UserDoesntHaveReservation = Error.NotFound(
        "CancelReservation.UserDoesntHaveReservation",
        "User doesn't have a reservation for the given session");

    public static readonly Error ParticipantNotFound = Error.NotFound(
        "CancelReservation.ParticipantNotFound",
        "Participant not found");

    public static readonly Error ParticipantExpectedToHaveReservation = Error.Unexpected(
        "CancelReservation.ParticipantExpectedToHaveReservation",
        "Participant expected to have reservation to session");
}
