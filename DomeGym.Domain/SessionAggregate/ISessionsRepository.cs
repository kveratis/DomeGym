namespace DomeGym.Domain.SessionAggregate;

public interface ISessionsRepository
{
    Task AddSessionAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
    Task<List<Session>> ListByIdsAsync(
        IReadOnlyList<Guid> sessionIds,
        DateTime? startDateTime = null,
        DateTime? endDateTime = null,
        List<SessionCategory>? categories = null);

    Task<List<Session>> ListByGymIdAsync(
        Guid gymId,
        DateTime? startDateTime = null,
        DateTime? endDateTime = null,
        List<SessionCategory>? categories = null);

    Task UpdateAsync(Session session);

    Task<List<Session>> ListByRoomIdAsync(Guid roomId);
    Task RemoveRangeAsync(List<Session> sessions);
}
