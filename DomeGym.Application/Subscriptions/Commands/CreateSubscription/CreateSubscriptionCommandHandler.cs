using DomeGym.Domain.AdminAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Subscriptions.Commands.CreateSubscription;

public sealed class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly IAdminsRepository _adminsRepository;

    public CreateSubscriptionCommandHandler(IAdminsRepository adminsRepository)
    {
        _adminsRepository = adminsRepository;
    }
    
    public async Task<ErrorOr<Subscription>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var admin = await _adminsRepository.GetByIdAsync(command.AdminId);

        if (admin is null)
        {
            return CreateSubscriptionErrors.AdminNotFound;
        }

        if (admin.SubscriptionId is not null)
        {
            return CreateSubscriptionErrors.AdminAlreadyHasActiveSubscription;
        }

        var subscription = new Subscription(command.SubscriptionType, command.AdminId);
        admin.SetSubscription(subscription);

        await _adminsRepository.UpdateAsync(admin);

        return subscription;
    }
}
