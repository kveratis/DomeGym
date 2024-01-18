using System.Text.Json.Serialization;
using DomeGym.Application.Subscriptions.Commands.CreateSubscription;
using DomeGym.Application.Subscriptions.Queries.ListSubscriptions;
using DomeGym.Domain.SubscriptionAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomeGym.Api.Controllers;

public record CreateSubscriptionRequest(SubscriptionType SubscriptionType, Guid AdminId);
public record SubscriptionResponse(Guid Id, SubscriptionType SubscriptionType);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionType
{
    Free,
    Starter,
    Pro
}

[Route("subscriptions")]
public sealed class SubscriptionsController : ApiController
{
    private readonly ISender _sender;

    public SubscriptionsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSubscription(CreateSubscriptionRequest request)
    {
        if (!Domain.SubscriptionAggregate.SubscriptionType.TryFromName(
                request.SubscriptionType.ToString(),
                out var subscriptionType))
        {
            return Problem("Invalid subscription type", statusCode: StatusCodes.Status400BadRequest);
        }

        var command = new CreateSubscriptionCommand(subscriptionType, request.AdminId);

        var createSubscriptionResult = await _sender.Send(command);

        return createSubscriptionResult.Match(
            subscription => Ok(new SubscriptionResponse(
                subscription.Id,
                ToDto(subscription.SubscriptionType))),
            Problem);
    }
    
    [HttpGet]
    public async Task<IActionResult> ListSubscriptions()
    {
        // TODO: get user/admin id from token, for now, return all
        var query = new ListSubscriptionsQuery();

        var listSubscriptionsResult = await _sender.Send(query);

        return listSubscriptionsResult.Match(
            subscriptions => Ok(subscriptions.ConvertAll(subscription => new SubscriptionResponse(
                subscription.Id,
                ToDto(subscription.SubscriptionType)))),
            Problem);
    }
    
    private static SubscriptionType ToDto(Domain.SubscriptionAggregate.SubscriptionType subscriptionType)
    {
        return subscriptionType.Name switch
        {
            nameof(Domain.SubscriptionAggregate.SubscriptionType.Free) => SubscriptionType.Free,
            nameof(Domain.SubscriptionAggregate.SubscriptionType.Starter) => SubscriptionType.Starter,
            nameof(Domain.SubscriptionAggregate.SubscriptionType.Pro) => SubscriptionType.Pro,
            _ => throw new InvalidOperationException(),
        };
    }
}