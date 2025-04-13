using Microsoft.Extensions.Logging;
using RxAspireApp.Domain.Strategy.Model;
using RxDemo.Common.Strategy.DTO;

namespace RxAspireApp.Domain.Strategy.Services;

public interface ITradeExecutionService
{
    void bloombergInstructionExecutionRetry(TradeStrategy strategy);
}

public class TradeExecutionService : ITradeExecutionService
{
    private readonly IBloombergService _bloombergService;
    private readonly ILogger<TradeExecutionService> _logger;

    public TradeExecutionService(IBloombergService bloombergService, ILogger<TradeExecutionService> logger)
    {
        _bloombergService = bloombergService;
        _logger = logger;
    }


    private int _maxExecutionAttempts = 5;

    public void bloombergInstructionExecutionRetry(TradeStrategy strategy)
    {
        bool tradeExecutedSuccessfully = false;
        int executionAttempts = 0;
        while (!tradeExecutedSuccessfully)
        {
            try
            {
                executionAttempts++;
                if (strategy.StrategyDetails.Instruction == BuySell.Buy)
                {
                    _bloombergService.Buy(strategy.StrategyDetails.Ticker, strategy.StrategyDetails.Quantity);
                    _logger.LogInformation($"BUY Strategy discovered : {strategy.ToString()} - {strategy.Execute()} - ExecutionAttempts={executionAttempts}");
                    tradeExecutedSuccessfully = true;
                }
                if (strategy.StrategyDetails.Instruction == BuySell.Sell)
                {
                    _bloombergService.Sell(strategy.StrategyDetails.Ticker, strategy.StrategyDetails.Quantity);
                    _logger.LogInformation($"SELL Strategy discovered : {strategy.ToString()} - {strategy.Execute()} - ExecutionAttempts={executionAttempts}");
                    tradeExecutedSuccessfully = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Trade execution failure", ex);
                if (executionAttempts == _maxExecutionAttempts)
                {
                    _logger.LogWarning($"Trade execution failure attempt limit [{_maxExecutionAttempts}] reached {strategy.ToString()}", ex);
                    strategy.ExecutionFailureReason = ex.Message;
                    return;
                }
            }
        }
    }
}
