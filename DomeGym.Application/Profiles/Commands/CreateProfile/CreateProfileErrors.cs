using ErrorOr;

namespace DomeGym.Application.Profiles.Commands.CreateProfile;

public static class CreateProfileErrors
{
    public static readonly Error UserAlreadyHasAdminProfile = Error.Conflict(
        "CreateProfile.UserAlreadyHasAdminProfile",
        "User already has an admin profile");

    public static readonly Error UserAlreadyHasParticipantProfile = Error.Conflict(
        "CreateProfile.UserAlreadyHasParticipantProfile",
        "User already has a participant profile");

    public static readonly Error UserAlreadyHasTrainerProfile = Error.Conflict(
        "CreateProfile.UserAlreadyHasTrainerProfile",
        "User already has a trainer profile");
}
