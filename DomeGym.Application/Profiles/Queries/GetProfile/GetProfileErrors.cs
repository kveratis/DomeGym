using ErrorOr;

namespace DomeGym.Application.Profiles.Queries.GetProfile;

public static class GetProfileErrors
{
    public static readonly Error UnexpectedProfileType = Error.Unexpected(
        "GetProfile.UnexpectedProfileType",
        "Unexpected profile type");
}
