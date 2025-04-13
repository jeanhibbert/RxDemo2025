using Microsoft.AspNetCore.SignalR;

namespace Pricing.Services;

public interface IContextHolder
{
    IHubCallerClients PricingHubClient    { get; set; }
}

public class ContextHolder : IContextHolder
{
    public IHubCallerClients PricingHubClient { get; set; }
}
