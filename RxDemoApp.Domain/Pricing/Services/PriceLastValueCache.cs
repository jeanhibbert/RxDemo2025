using RxDemo.Common.Pricing.DTO;
using System.Collections.Concurrent;

namespace Pricing.Services;

public interface IPriceLastValueCache
{
    PriceDto GetLastValue(string currencyPair);
    void StoreLastValue(PriceDto price);
}

public class PriceLastValueCache : IPriceLastValueCache
{
    private readonly ConcurrentDictionary<string, PriceDto> _lastValueCache = new ConcurrentDictionary<string, PriceDto>();

    public PriceDto GetLastValue(string currencyPair)
    {
        PriceDto price;
        if (_lastValueCache.TryGetValue(currencyPair, out price))
        {
            return price;
        }
        throw new InvalidOperationException(string.Format("Currency pair {0} has not been initialized in last value cache", currencyPair));
    }

    public void StoreLastValue(PriceDto price)
    {
        _lastValueCache.AddOrUpdate(price.Symbol, _ => price, (s, p) => price);
    }
}
