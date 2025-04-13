using Microsoft.AspNetCore.SignalR.Client;
using RxAspireApp.Client.Domain.Transport;
using RxDemo.Common.Pricing;
using RxDemo.Common.Pricing.DTO;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxAspireApp.Client.Domain.ServiceClients;

public interface IPricingServiceClient
{
    IObservable<PriceDto> GetSpotStream(string currencyPair);
}

public class PricingServiceClient : ServiceClientBase, IPricingServiceClient
{
    public PricingServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    public IObservable<PriceDto> GetSpotStream(string currencyPair)
    {
        if (string.IsNullOrEmpty(currencyPair)) throw new ArgumentException("currencyPair");

        return GetResilientStream(connection => GetSpotStreamForConnection(currencyPair, connection.PricingHubConnection), TimeSpan.FromSeconds(5));
    }

    private static IObservable<PriceDto> GetSpotStreamForConnection(string currencyPair, HubConnection pricingHubProxy)
    {
        return Observable.Create<PriceDto>(observer =>
        {
            // subscribe to price stream
            var priceSubscription = pricingHubProxy.On<PriceDto>(ServiceConstants.Client.OnNewPrice, p =>
            {
                if (p.Symbol == currencyPair)
                {
                    observer.OnNext(p);
                }
            });

            // request subscription
            Debug.WriteLine($"Sending price subscription for currency pair {currencyPair}");
            SendSubscription(currencyPair, pricingHubProxy)
                .Subscribe(
                    _ => Debug.WriteLine($"Subscribed to {currencyPair}"),
                    observer.OnError);


            var unsubscriptionDisposable = Disposable.Create(() =>
            {
                // Unsubscribe on dispose
                Debug.WriteLine($"Sending price unsubscription for currency pair {currencyPair}");
                SendUnsubscription(currencyPair, pricingHubProxy)
                    .Subscribe(
                        _ => Debug.WriteLine("Unsubscribed from {0}", currencyPair),
                        ex =>
                            Debug.WriteLine("An error occured while sending unsubscription request for {0}:{1}", currencyPair, ex.Message));
            });

            return new CompositeDisposable { priceSubscription, unsubscriptionDisposable };
        })
        .Publish()
        .RefCount();
    }

    private static IObservable<Unit> SendSubscription(string currencyPair, HubConnection pricingHubProxy)
    {
        return Observable.FromAsync(
            () => pricingHubProxy.SendAsync(ServiceConstants.Server.SubscribePriceStream,
            new PriceSubscriptionRequestDto { CurrencyPair = currencyPair }));
    }

    private static IObservable<Unit> SendUnsubscription(string currencyPair, HubConnection pricingHubProxy)
    {
        return Observable.FromAsync(
            () => pricingHubProxy.SendAsync(ServiceConstants.Server.UnsubscribePriceStream,
            new PriceSubscriptionRequestDto { CurrencyPair = currencyPair }));
    }
}