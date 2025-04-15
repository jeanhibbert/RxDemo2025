using Pricing.Services;

namespace RxAspireApp.ApiService.Pricing.Extensions;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddHub(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(setup =>
        {
            setup.AddDefaultPolicy(policy =>
                policy.SetIsOriginAllowed(_ => true).AllowCredentials().AllowAnyHeader().AllowAnyMethod());
        });
        serviceCollection.AddSignalR();

        return serviceCollection;
    }

    public static IServiceCollection AddPricingServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IContextHolder, ContextHolder>();
        serviceCollection.AddSingleton<ICurrencyPairHolder, CurrencyPairHolder>();
        serviceCollection.AddSingleton<IPriceLastValueCache, PriceLastValueCache>();
        serviceCollection.AddSingleton<IPricePublisher, PricePublisher>();

        serviceCollection.AddSingleton<IFeedGenerator, FeedGenerator>();
        return serviceCollection;
    }
}