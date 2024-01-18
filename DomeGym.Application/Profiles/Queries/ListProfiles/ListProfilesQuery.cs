using DomeGym.Domain;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Profiles.Queries.ListProfiles;

public sealed record ListProfilesQuery(Guid UserId) : IRequest<ErrorOr<List<Profile>>>;
