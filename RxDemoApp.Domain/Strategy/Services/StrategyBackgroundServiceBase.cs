using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RxAspireApp.Domain.Strategy.Services;

public abstract class StrategyBackgroundServiceBase : BackgroundService
{
    private readonly TimeSpan _tickFrequency;

    private readonly ILogger<StrategyBackgroundServiceBase> _logger;

    protected StrategyBackgroundServiceBase(TimeSpan tickFrequency, ILogger<StrategyBackgroundServiceBase> logger)
    {
        _tickFrequency = tickFrequency;
        _logger = logger;
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Trade Execution Service is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckStrategies().ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception was thrown whilst checking registered strategies.");
                throw;
            }

            await Task.Delay(_tickFrequency, stoppingToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        _logger.LogInformation("Trade Execution Service is stopping.");
    }

    protected abstract Task CheckStrategies();
}
