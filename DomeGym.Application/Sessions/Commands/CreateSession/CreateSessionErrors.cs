using ErrorOr;

namespace DomeGym.Application.Sessions.Commands.CreateSession;

public static class CreateSessionErrors
{
    public static readonly Error RoomNotFound = Error.NotFound(
        "CreateSession.RoomNotFound", 
        "Room not found");

    public static readonly Error TrainerNotFound = Error.NotFound(
        "CreateSession.TrainerNotFound", 
        "Trainer not found");

    public static readonly Error InvalidDateAndTime = Error.Validation(
        "CreateSession.InvalidDateAndTime", 
        "Invalid date and time");

    public static readonly Error TrainersCalendarIsNotFreeForSession = Error.Validation(
        "CreateSession.TrainersCalendarIsNotFreeForSession", 
        "Trainer's calendar is not free for the entire session duration");
}
