using DomeGym.Domain.RoomAggregate;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Sessions.Queries.GetSession;

public sealed class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, ErrorOr<Session>>
{
    public static readonly Error RoomNotFound = Error.NotFound(
        "GetSessionQueryHandler.RoomNotFound", 
        "Room not found");
    
    public static readonly Error SessionNotFound = Error.NotFound(
        "GetSessionQueryHandler.SessionNotFound", 
        "Session not found");
    
    private readonly IRoomsRepository _roomsRepository;
    private readonly ISessionsRepository _sessionsRepository;

    public GetSessionQueryHandler(ISessionsRepository sessionsRepository, IRoomsRepository roomsRepository)
    {
        _sessionsRepository = sessionsRepository;
        _roomsRepository = roomsRepository;
    }
    
    public async Task<ErrorOr<Session>> Handle(GetSessionQuery query, CancellationToken cancellationToken)
    {
        var room = await _roomsRepository.GetByIdAsync(query.RoomId);

        if (room is null)
        {
            return RoomNotFound;
        }

        if (!room.HasSession(query.SessionId))
        {
            return SessionNotFound;
        }

        if (await _sessionsRepository.GetByIdAsync(query.SessionId) is not Session session)
        {
            return SessionNotFound;
        }

        return session;
    }
}