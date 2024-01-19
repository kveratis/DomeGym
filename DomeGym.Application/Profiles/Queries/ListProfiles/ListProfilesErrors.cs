using ErrorOr;

namespace DomeGym.Application.Profiles.Queries.ListProfiles;

public static class ListProfilesErrors
{
    public static readonly Error UserNotFound = Error.NotFound(
        "ListProfiles.UserNotFound",
        "User not found");
}
