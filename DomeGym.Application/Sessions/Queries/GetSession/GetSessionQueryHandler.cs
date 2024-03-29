using DomeGym.Domain.RoomAggregate;
using DomeGym.Domain.SessionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Sessions.Queries.GetSession;

public sealed class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, ErrorOr<Session>>
{
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
            return GetSessionErrors.RoomNotFound;
        }

        if (!room.HasSession(query.SessionId))
        {
            return GetSessionErrors.SessionNotFound;
        }

        if (await _sessionsRepository.GetByIdAsync(query.SessionId) is not Session session)
        {
            return GetSessionErrors.SessionNotFound;
        }

        return session;
    }
}
