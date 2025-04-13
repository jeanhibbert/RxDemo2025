using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RxAspireApp.Domain.Strategy.Repositories;
using RxDemo.Common.Strategy.DTO;
using System.Reactive.Linq;

namespace RxAspireApp.Domain.Strategy.Services;

public interface IStrategyManagementService : IHostedService
{
    IList<StrategyDetailsDto> GetAllStrategies();
    IList<string> RegisterStrategy(StrategyDetailsDto strategyDetails);
    bool UnregisterStrategy(string strategyId);
}

public class StrategyManagementService : StrategyBackgroundServiceBase, IStrategyManagementService
{
    private const int TickFrequencyMilliseconds = ServiceConstants.Server.TickFrequencyMilliseconds;
    private readonly ILogger<StrategyManagementService> _logger;
    private readonly ITickerDataRepository _tickerDataRepository;
    private readonly ITradeStrategyRepository _tradeStrategyRepository;
    private readonly ITradeExecutionService _tradeExecutionService;

    public StrategyManagementService(ILogger<StrategyManagementService> logger, 
        ITickerDataRepository tickerDataRepository,
        ITradeStrategyRepository tradeStrategyRepository,
        ITradeExecutionService tradeExecutionService)
        : base(TimeSpan.FromMilliseconds(TickFrequencyMilliseconds), logger)
    {
        _logger = logger;
        _tickerDataRepository = tickerDataRepository;
        _tradeStrategyRepository = tradeStrategyRepository;
        _tradeExecutionService = tradeExecutionService;
    }

    public IList<string> RegisterStrategy(StrategyDetailsDto strategyDetails)
    {
        if (!strategyDetails.Ticker.IsValidTicker())
            throw new ArgumentException("Invalid ticker provided");
        
        return _tradeStrategyRepository.AddTradeStrategy(strategyDetails, 
            _tickerDataRepository.RegisterTicker(strategyDetails.Ticker).PriceStream);
    }

    public IList<StrategyDetailsDto> GetAllStrategies()
    {
        return _tradeStrategyRepository.TradeStrategyDetails;
    }

    public bool UnregisterStrategy(string strategyId)
    {
        return _tradeStrategyRepository.RemoveStrategy(strategyId);
    }

    protected override Task CheckStrategies()
    {
        _logger.LogInformation($"Checking strategies...");

        var tickerPriceStream = from tickerData in _tickerDataRepository.GetTickerDataStream()
                                    //where tickerData.Ticker == "MSFT"
                                    from price in tickerData.PriceStream
                                    select price;
                 
        var validTradeStrategy = from price in tickerPriceStream
                     from tradeStrategy in _tradeStrategyRepository.TradeStrategies
                     where tradeStrategy.HasExecuted == false 
                        && tradeStrategy.ExecutionFailureReason == null
                        && tradeStrategy.StrategyDetails.Ticker == price.Ticker
                        && tradeStrategy.IsReadyToExecute(price)
                     select tradeStrategy;

        validTradeStrategy.Subscribe(strategy =>
        {
            _tradeExecutionService.bloombergInstructionExecutionRetry(strategy);
        });

        LogTradeStrategyStatus();
        _tickerDataRepository.DisposeUnusedTickers();

        return Task.CompletedTask;
    }

    private void LogTradeStrategyStatus()
    {
        var outstandingTotal = _tradeStrategyRepository.TradeStrategies.Where(x => !x.HasExecuted).Count();
        var executedTotal = _tradeStrategyRepository.TradeStrategies.Where(x => x.HasExecuted).Count();
        var failedTotal = _tradeStrategyRepository.TradeStrategies.Where(x => x.ExecutionFailureReason != null).Count();
        _logger.LogInformation($"Strategies checked... OUSTANDING: {outstandingTotal} | EXECUTED: {executedTotal} | FAILED: {failedTotal}");
    }

    
}
