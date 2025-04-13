using Microsoft.Extensions.Logging;
using RxAspireApp.Domain.Strategy.Model;
using RxAspireApp.Domain.Strategy.Repositories;
using RxAspireApp.Domain.Strategy.Services;

namespace RxAspireApp.Domain.Strategy.Factories;

public interface ITickerDataFactory
{
    ITickerData Create(string ticker);
}

public class TickerDataFactory : ITickerDataFactory
{
    private readonly ILogger<TickerDataFactory> _logger;

    private readonly IBloombergService _bloombergService;
    public TickerDataFactory(ILogger<TickerDataFactory> logger, IBloombergService bloombergService)
    {
        _logger = logger;
        _bloombergService = bloombergService;
    }

    public ITickerData Create(string ticker)
    {
        var tickerData = new TickerData(
            ticker,
            new PriceGenerator(_logger, _bloombergService));

        return tickerData;
    }
}
