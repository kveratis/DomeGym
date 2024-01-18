using DomeGym.Domain.GymAggregate;
using DomeGym.Domain.RoomAggregate;
using DomeGym.Domain.SubscriptionAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ErrorOr<Room>>
{
    public static readonly Error GymNotFound = Error.NotFound(
        "CreateRoomCommandHandler.GymNotFound", 
        "Gym not found");
    
    public static readonly Error SubscriptionNotFound = Error.NotFound(
        "CreateRoomCommandHandler.SubscriptionNotFound", 
        "Subscription not found");
    
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IGymsRepository _gymsRepository;

    public CreateRoomCommandHandler(ISubscriptionsRepository subscriptionsRepository, IGymsRepository gymsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _gymsRepository = gymsRepository;
    }
    
    public async Task<ErrorOr<Room>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.GymId);

        if (gym is null)
        {
            return GymNotFound;
        }

        var subscription = await _subscriptionsRepository.GetByIdAsync(gym.SubscriptionId);

        if (subscription is null)
        {
            return SubscriptionNotFound;
        }

        var room = new Room(
            name: command.RoomName,
            maxDailySessions: subscription.GetMaxDailySessions(),
            gymId: gym.Id);

        var addGymResult = gym.AddRoom(room);

        if (addGymResult.IsError)
        {
            return addGymResult.Errors;
        }

        await _gymsRepository.UpdateAsync(gym);

        return room;
    }
}