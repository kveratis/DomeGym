namespace DomeGym.Domain.SubscriptionAggregate;

public interface ISubscriptionsRepository
{
    Task AddSubscriptionAsync(Subscription subscription);
    Task<bool> ExistsAsync(Guid id);
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<List<Subscription>> ListAsync();
    Task UpdateAsync(Subscription subscription);
}
