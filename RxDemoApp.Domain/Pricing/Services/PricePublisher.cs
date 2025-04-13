using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RxDemo.Common.Pricing.DTO;

namespace Pricing.Services;

public interface IPricePublisher
{
    Task Publish(PriceDto price);
    long TotalPricesPublished { get; }
}

public class PricePublisher(IContextHolder contextHolder,
        ILogger<PricePublisher> logger) : IPricePublisher
{
    private long _totalUpdatesPublished;

    public async Task Publish(PriceDto price)
    {
        var context = contextHolder.PricingHubClient;
        if (context == null) return;

        _totalUpdatesPublished++;
        var groupName = string.Format(PricingHub.PriceStreamGroupPattern, price.Symbol);
        try
        {
            await context.Group(groupName).SendAsync("OnNewPrice", price);
            logger.LogDebug("Published price to group {0}: {1}", groupName, price);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while publishing price to group {GroupName}: {Price}", groupName, price);
        }
    }

    public long TotalPricesPublished { get { return _totalUpdatesPublished; } }
}