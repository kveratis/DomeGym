using ErrorOr;

namespace DomeGym.Application.Participants.Queries.ListParticipantSessions;

public static class ListParticipantErrors
{
    public static readonly Error ParticipantNotFound = Error.NotFound(
        "ListParticipant.ParticipantNotFound",
        "Participant not found");
}
