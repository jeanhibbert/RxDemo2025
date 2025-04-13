using Microsoft.AspNetCore.SignalR.Client;
using RxDemo.Common.Pricing;
using RxDemo.Common.Pricing.DTO;

namespace RxAspireApp.Client.Domain.ServiceClients;

public class TestHubClient : IAsyncDisposable
{
    private readonly string _hubUrl;
    private bool _initialized = false;
    private HubConnection _hubConnection = null!;

    public event Action<PriceDto>? PriceReceived;

    public TestHubClient(string hubUrl)
    {
        _hubUrl = hubUrl;
    }

    public async Task Initialize()
    {
        if (!_initialized)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<PriceDto>("OnNewPrice", (priceDto) =>
                PriceReceived?.Invoke(priceDto));

            await _hubConnection.StartAsync();

            await SendSubscriptionRequest("EURUSD");

            _initialized = true;
        }
    }


    public Task SendSubscriptionRequest(string currencyPair) =>
       _hubConnection.SendAsync(ServiceConstants.Server.SubscribePriceStream,
            new PriceSubscriptionRequestDto { CurrencyPair = currencyPair });

    public Task SendUnSubscriptionRequest(string currencyPair) =>
       _hubConnection.SendAsync(ServiceConstants.Server.UnsubscribePriceStream,
            new PriceSubscriptionRequestDto { CurrencyPair = currencyPair });

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
            _initialized = false;
        }
    }
}