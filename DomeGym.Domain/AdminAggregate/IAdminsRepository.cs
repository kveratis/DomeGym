namespace DomeGym.Domain.AdminAggregate;

public interface IAdminsRepository
{
    Task AddAdminAsync(Admin participant);
    Task<Profile?> GetProfileByUserIdAsync(Guid userId);
    Task<Admin?> GetByIdAsync(Guid adminId);
    Task UpdateAsync(Admin admin);
}
