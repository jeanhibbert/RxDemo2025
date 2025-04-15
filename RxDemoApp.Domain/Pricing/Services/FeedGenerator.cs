using RxDemo.Common.Pricing.DTO;
using System.Reactive.Linq;

namespace Pricing.Services;

public interface IFeedGenerator
{
    void Start();
    void UpdateFrequency();
}

public class FeedGenerator : IFeedGenerator, IDisposable
{
    private readonly ICurrencyPairHolder _currencyPairHolder;
    private readonly IPricePublisher _pricePublisher;
    private readonly IPriceLastValueCache _priceLastValueCache;
    private readonly Random _random;
    private IDisposable _subscription;

    public FeedGenerator(
        ICurrencyPairHolder currencyPairHolder,
        IPricePublisher pricePublisher,
        IPriceLastValueCache priceLastValueCache)
    {
        _currencyPairHolder = currencyPairHolder;
        _pricePublisher = pricePublisher;
        _priceLastValueCache = priceLastValueCache;
        _random = new Random(_currencyPairHolder.GetHashCode());
    }

    public void Start()
    {
        PopulateLastValueCache();
        UpdateFrequency();
    }

    public void UpdateFrequency()
    {
        _subscription?.Dispose();

        var periodMs = 1000.0;

        _subscription = Observable.Interval(TimeSpan.FromMilliseconds(periodMs))
            .Subscribe(_ => UpdateCurrencyPairs());
    }

    private void PopulateLastValueCache()
    {
        foreach (var currencyPairInfo in _currencyPairHolder.GetAllCurrencyPairs())
        {
            var mid = _currencyPairHolder.GetSampleRate(
                currencyPairInfo.CurrencyPair.Symbol,
                currencyPairInfo.CurrencyPair.CounterParty);

            var initialQuote = new PriceDto
            {
                Symbol = currencyPairInfo.CurrencyPair.Symbol,
                CounterParty = currencyPairInfo.CurrencyPair.CounterParty,
                QuoteId = 0,
                Mid = mid
            };

            _priceLastValueCache.StoreLastValue(currencyPairInfo.NextQuote(initialQuote));
        }
    }

    private void UpdateCurrencyPairs()
    {
        var activePairs = _currencyPairHolder.GetAllCurrencyPairs().Where(cp => cp.Enabled).ToList();

        if (activePairs.Count == 0)
            return;

        for (int i = 0; i < activePairs.Count; i++)
        {
            var randomCurrencyPairInfo = activePairs[_random.Next(0, activePairs.Count)];
            var lastPrice = _priceLastValueCache.GetLastValue(
                randomCurrencyPairInfo.CurrencyPair.Symbol, 
                randomCurrencyPairInfo.CurrencyPair.CounterParty);

            var newPrice = randomCurrencyPairInfo.NextQuote(lastPrice);

            _priceLastValueCache.StoreLastValue(newPrice);
            _pricePublisher.Publish(newPrice);
        }
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}
