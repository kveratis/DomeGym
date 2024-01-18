using DomeGym.Domain;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Profiles.Commands.CreateProfile;

public sealed record CreateProfileCommand(ProfileType ProfileType, Guid UserId)
    : IRequest<ErrorOr<Guid>>;
