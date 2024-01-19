using DomeGym.Domain;
using DomeGym.Domain.RoomAggregate;
using DomeGym.Domain.SessionAggregate;
using DomeGym.Domain.TrainerAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Sessions.Commands.CreateSession;

public sealed class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, ErrorOr<Session>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly ITrainersRepository _trainersRepository;

    public CreateSessionCommandHandler(ITrainersRepository trainersRepository, IRoomsRepository roomsRepository)
    {
        _trainersRepository = trainersRepository;
        _roomsRepository = roomsRepository;
    }
    
    public async Task<ErrorOr<Session>> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var room = await _roomsRepository.GetByIdAsync(command.RoomId);

        if (room is null)
        {
            return CreateSessionErrors.RoomNotFound;
        }

        var trainer = await _trainersRepository.GetByIdAsync(command.TrainerId);

        if (trainer is null)
        {
            return CreateSessionErrors.TrainerNotFound;
        }

        var createTimeRangeResult = TimeRange.FromDateTimes(command.StartDateTime, command.EndDateTime);

        if (createTimeRangeResult.IsError && createTimeRangeResult.FirstError.Type == ErrorType.Validation)
        {
            return CreateSessionErrors.InvalidDateAndTime;
        }

        if (!trainer.IsTimeSlotFree(DateOnly.FromDateTime(command.StartDateTime), createTimeRangeResult.Value))
        {
            return CreateSessionErrors.TrainersCalendarIsNotFreeForSession;
        }

        var session = new Session(
            name: command.Name,
            description: command.Description,
            maxParticipants: command.MaxParticipants,
            roomId: command.RoomId,
            trainerId: command.TrainerId,
            date: DateOnly.FromDateTime(command.StartDateTime),
            time: createTimeRangeResult.Value,
            categories: command.Categories);

        var scheduleSessionResult = room.ScheduleSession(session);

        if (scheduleSessionResult.IsError)
        {
            return scheduleSessionResult.Errors;
        }

        await _roomsRepository.UpdateAsync(room);

        return session;
    }
}
