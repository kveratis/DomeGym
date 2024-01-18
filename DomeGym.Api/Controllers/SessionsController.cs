using DomeGym.Application.Sessions.Commands.CreateSession;
using DomeGym.Application.Sessions.Queries.GetSession;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomeGym.Api.Controllers;

public sealed record CreateSessionRequest(
    string Name,
    string Description,
    int MaxParticipants,
    DateTime StartDateTime,
    DateTime EndDateTime,
    Guid TrainerId,
    List<string> Categories);

[Route("rooms/{roomId:guid}/sessions")]
public sealed class SessionsController : ApiController
{
    private readonly ISender _sender;

    public SessionsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSession(
        CreateSessionRequest request,
        Guid roomId)
    {
        var categoriesToDomainResult = SessionCategoryUtils.ToDomain(request.Categories);

        if (categoriesToDomainResult.IsError)
        {
            return Problem(categoriesToDomainResult.Errors);
        }

        var command = new CreateSessionCommand(
            roomId,
            request.Name,
            request.Description,
            request.MaxParticipants,
            request.StartDateTime,
            request.EndDateTime,
            request.TrainerId,
            categoriesToDomainResult.Value);

        var createSessionResult = await _sender.Send(command);

        return createSessionResult.Match(
            session => CreatedAtAction(
                nameof(GetSession),
                new { roomId, SessionId = session.Id },
                new SessionResponse(
                    session.Id,
                    session.Name,
                    session.Description,
                    session.NumParticipants,
                    session.MaxParticipants,
                    session.Date.ToDateTime(session.Time.Start),
                    session.Date.ToDateTime(session.Time.End),
                    session.Categories.Select(category => category.Name).ToList())),
            Problem);
    }
    
    [HttpGet("{sessionId:guid}")]
    public async Task<IActionResult> GetSession(
        Guid roomId,
        Guid sessionId)
    {
        var query = new GetSessionQuery(
            roomId,
            sessionId);

        var getSessionResult = await _sender.Send(query);

        return getSessionResult.Match(
            session => Ok(new SessionResponse(
                session.Id,
                session.Name,
                session.Description,
                session.NumParticipants,
                session.MaxParticipants,
                session.Date.ToDateTime(session.Time.Start),
                session.Date.ToDateTime(session.Time.End),
                session.Categories.Select(category => category.Name).ToList())),
            Problem);
    }
}