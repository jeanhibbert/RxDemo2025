using RxAspireApp.Domain.Strategy.Factories;
using RxAspireApp.Domain.Strategy.Repositories;
using RxAspireApp.Domain.Strategy.Services;

namespace RxAspireApp.ApiService.Strategy.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddStrategyServices(this IServiceCollection services)
    {
        services.AddHostedService<StrategyManagementService>();
        services.AddSingleton<IHostedServiceAccessor<IStrategyManagementService>, HostedServiceAccessor<IStrategyManagementService>>();
        services.AddSingleton<IBloombergService, BloombergService>();
        services.AddSingleton<ITickerDataRepository, TickerDataRepository>();
        services.AddSingleton<ITradeStrategyRepository, TradeStrategyRepository>();
        services.AddSingleton<ITickerDataFactory, TickerDataFactory>();
        services.AddSingleton<ITradeExecutionService, TradeExecutionService>();
        return services;
    }
}
