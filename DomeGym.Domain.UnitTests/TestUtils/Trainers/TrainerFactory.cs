using DomeGym.Domain.UnitTests.TestConstants;

namespace DomeGym.Domain.UnitTests.TestUtils.Trainers;

internal static class TrainerFactory
{
    public static Trainer CreateTrainer(
        Guid? userId = null,
        Guid? id = null)
    {
        return new Trainer(
            userId: userId ?? Constants.User.Id,
            id: id ?? Constants.Trainer.Id);
    }
}