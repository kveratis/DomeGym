using ErrorOr;

namespace DomeGym.Application.Sessions.Queries.GetSession;

public static class GetSessionErrors
{
    public static readonly Error RoomNotFound = Error.NotFound(
        "GetSession.RoomNotFound", 
        "Room not found");

    public static readonly Error SessionNotFound = Error.NotFound(
        "GetSession.SessionNotFound", 
        "Session not found");
}
