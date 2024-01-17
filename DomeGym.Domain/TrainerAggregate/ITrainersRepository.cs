namespace DomeGym.Domain.TrainerAggregate;

public interface ITrainersRepository
{
    Task AddTrainerAsync(Trainer participant);
    Task<Trainer?> GetByIdAsync(Guid trainerId);
    Task<Profile?> GetProfileByUserIdAsync(Guid userId);
    Task UpdateAsync(Trainer trainer);
}