using ErrorOr;

namespace DomeGym.Application.Reservations.Commands.CreateReservation;

public static class CreateReservationErrors
{
    public static readonly Error SessionNotFound = Error.NotFound(
        "CreateReservation.SessionNotFound",
        "Session not found");

    public static readonly Error ParticipantAlreadyHasReservation = Error.NotFound(
        "CreateReservation.ParticipantAlreadyHasReservation", 
        "Participant already has reservation");

    public static readonly Error ParticipantNotFound = Error.NotFound(
        "CreateReservation.ParticipantNotFound",
        "Participant not found");

    public static readonly Error ParticipantNotExpectedToHaveReservation = Error.Unexpected(
        "CreateReservation.ParticipantNotExpectedToHaveReservation", 
        "Participant not expected to have reservation to session");

    public static readonly Error ParticipantCalendarNotFreeForSession = Error.Conflict(
        "CreateReservation.ParticipantCalendarNotFreeForSession", 
        "Participant's calendar is not free for the entire session duration");
}
