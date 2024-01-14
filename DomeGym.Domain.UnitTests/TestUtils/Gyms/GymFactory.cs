using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.UnitTests.TestConstants;

namespace DomeGym.Domain.UnitTests.TestUtils.Gyms;

internal static class GymFactory
{
    public static Gym CreateGym(
        int maxRooms = Constants.Subscription.MaxRoomsFreeTier,
        Guid? id = null)
    {
        return new Gym(
            maxRooms,
            subscriptionId: Constants.Subscription.Id,
            id: id ?? Constants.Gym.Id);
    }
}