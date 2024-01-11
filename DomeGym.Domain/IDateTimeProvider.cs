namespace DomeGym.Domain;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}