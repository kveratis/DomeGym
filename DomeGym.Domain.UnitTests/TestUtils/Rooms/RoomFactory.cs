using DomeGym.Domain.RoomAggregate;
using DomeGym.Domain.UnitTests.TestConstants;

namespace DomeGym.Domain.UnitTests.TestUtils.Rooms;

internal static class RoomFactory
{
    public static Room CreateRoom(
        int maxDailySessions = Constants.Room.MaxDailySessions,
        Guid? gymId = null,
        Guid? id = null)
    {
        return new Room(
            maxDailySessions: maxDailySessions,
            gymId: gymId ?? Constants.Gym.Id,
            id: id ?? Constants.Room.Id);
    }
}