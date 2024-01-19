using DomeGym.Domain;
using DomeGym.Domain.AdminAggregate;
using DomeGym.Domain.ParticipantAggregate;
using DomeGym.Domain.TrainerAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Profiles.Commands.CreateProfile;

public sealed class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ErrorOr<Guid>>
{
    private readonly IAdminsRepository _adminsRepository;
    private readonly ITrainersRepository _trainersRepository;
    private readonly IParticipantsRepository _participantsRepository;

    public CreateProfileCommandHandler(IAdminsRepository adminsRepository, ITrainersRepository trainersRepository, IParticipantsRepository participantsRepository)
    {
        _adminsRepository = adminsRepository;
        _trainersRepository = trainersRepository;
        _participantsRepository = participantsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        return command.ProfileType.Name switch
        {
            nameof(ProfileType.Admin) => await CreateAdminAsync(command.UserId),
            nameof(ProfileType.Trainer) => await CreateTrainerAsync(command.UserId),
            nameof(ProfileType.Participant) => await CreateParticipantAsync(command.UserId),
            _ => throw new InvalidOperationException()
        };
    }

    private async Task<ErrorOr<Guid>> CreateAdminAsync(Guid userId)
    {
        if (await _adminsRepository.GetProfileByUserIdAsync(userId) is not null)
        {
            return CreateProfileErrors.UserAlreadyHasAdminProfile;
        }

        var admin = new Admin(userId);
        await _adminsRepository.AddAdminAsync(admin);

        return admin.Id;
    }

    private async Task<ErrorOr<Guid>> CreateParticipantAsync(Guid userId)
    {
        if (await _participantsRepository.GetProfileByUserIdAsync(userId) is not null)
        {
            return CreateProfileErrors.UserAlreadyHasParticipantProfile;
        }

        var participant = new Participant(userId);
        await _participantsRepository.AddParticipantAsync(participant);

        return participant.Id;
    }

    private async Task<ErrorOr<Guid>> CreateTrainerAsync(Guid userId)
    {
        if (await _trainersRepository.GetProfileByUserIdAsync(userId) is not null)
        {
            return CreateProfileErrors.UserAlreadyHasTrainerProfile;
        }

        var trainer = new Trainer(userId);
        await _trainersRepository.AddTrainerAsync(trainer);

        return trainer.Id;
    }
}
