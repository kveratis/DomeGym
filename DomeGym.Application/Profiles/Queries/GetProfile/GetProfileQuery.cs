using DomeGym.Domain;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Profiles.Queries.GetProfile;

public sealed record GetProfileQuery(Guid UserId, ProfileType ProfileType) : IRequest<ErrorOr<Profile?>>;
