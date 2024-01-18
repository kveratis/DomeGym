using DomeGym.Domain.RoomAggregate;
using ErrorOr;
using MediatR;

namespace DomeGym.Application.Rooms.Queries.ListRooms;

public sealed record ListRoomsQuery(Guid GymId) : IRequest<ErrorOr<List<Room>>>;
