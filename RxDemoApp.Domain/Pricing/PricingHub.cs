using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pricing.Services;
using RxDemo.Common.Pricing;
using RxDemo.Common.Pricing.DTO;
using System.Diagnostics;

public class PricingHub(
        IPriceLastValueCache priceLastValueCache,
        ICurrencyPairHolder currencyPairRepository,
        IContextHolder contextHolder,
        ILogger<PricingHub> logger) : Microsoft.AspNetCore.SignalR.Hub
{

    public const string PriceStreamGroupPattern = "Pricing/{0}.{1}";
        
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    [HubMethodName(ServiceConstants.Server.SubscribePriceStream)]
    public async Task SubscribePriceStream(PriceSubscriptionRequestDto request)
    {
        contextHolder.PricingHubClient = Clients;

        logger.LogInformation("Received subscription request {0} from connection {1}", request, Context.ConnectionId);

        if (!currencyPairRepository.Exists(request.CurrencyPair, request.CounterParty))
        {
            Debug.WriteLine("Received a subscription request for an invalid currency pair '{0}', it was ignored.", request.CurrencyPair);
            return;
        }

        var groupName = string.Format(PriceStreamGroupPattern, request.CurrencyPair, request.CounterParty);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        logger.LogInformation("Connection {0} added to group '{1}'", Context.ConnectionId, groupName);
        
        var lastValue = priceLastValueCache.GetLastValue(request.CurrencyPair, request.CounterParty);
        await Clients.Caller.SendAsync("OnNewPrice", lastValue);
        logger.LogInformation("Snapshot published to {0}: {1}", Context.ConnectionId, lastValue);
    }

    [HubMethodName(ServiceConstants.Server.UnsubscribePriceStream)]
    public async Task UnsubscribePriceStream(PriceSubscriptionRequestDto request)
    {
        logger.LogInformation("Received unsubscription request {0} from connection {1}", request, Context.ConnectionId);

        if (!currencyPairRepository.Exists(request.CurrencyPair, request.CounterParty))
        {
            logger.LogInformation("Received an unsubscription request for an invalid currency pair '{0}', it was ignored.", request.CurrencyPair);
            return;
        }

        var groupName = string.Format(PriceStreamGroupPattern, request.CurrencyPair, request.CounterParty);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        logger.LogInformation("Connection {0} removed from group '{1}'", Context.ConnectionId, groupName);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        await base.OnDisconnectedAsync(exception);
    }
}