namespace DomeGym.Domain.ParticipantAggregate;

public interface IParticipantsRepository
{
    public Task AddParticipantAsync(Participant participant);
    Task<Participant?> GetByIdAsync(Guid id);
    public Task<Profile?> GetProfileByUserIdAsync(Guid userId);
    Task<List<Participant>> ListByIdsAsync(List<Guid> ids);
    Task UpdateAsync(Participant participant);
    Task UpdateRangeAsync(List<Participant> participants);
}
