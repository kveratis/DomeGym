namespace DomeGym.Domain.UnitTests.TestConstants;

internal static partial class Constants
{
    public static class Room
    {
        public static readonly Guid Id = Guid.NewGuid();
        public const int MaxDailySessions = Subscription.MaxDailySessionsFreeTier;
    }
}