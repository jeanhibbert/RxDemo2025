using RxDemo.Common.Pricing.DTO;

namespace Pricing.Services;

public interface ICurrencyPairHolder
{
    IEnumerable<CurrencyPairInfo> GetAllCurrencyPairs();
    bool Exists(string symbol, string counterParty);
    decimal GetSampleRate(string symbol, string counterParty);
}

public class CurrencyPairHolder : ICurrencyPairHolder
{
    private readonly Dictionary<string, CurrencyPairInfo> _currencyPairs = new Dictionary<string, CurrencyPairInfo>
        {
            {"EURUSD.JPM", NewCurrencyPair("EURUSD", "JPM", 4, 5, 1.3629m, true)},
            {"EURUSD.BAML", NewCurrencyPair("EURUSD", "BAML", 4, 5, 1.3629m, true)},
            {"USDJPY.JPM", NewCurrencyPair("USDJPY", "JPM", 2, 3, 102.14m, true)},
            {"USDJPY.BAML", NewCurrencyPair("USDJPY", "BAML", 2, 3, 102.14m, true)},
            {"GBPUSD.JPM", NewCurrencyPair("GBPUSD", "JPM", 4, 5, 1.6395m, true)},
            {"GBPUSD.BAML", NewCurrencyPair("GBPUSD", "BAML", 4, 5, 1.6395m, true)}
        };

    private static CurrencyPairInfo NewCurrencyPair(string symbol, string counterParty, int pipsPosition, int ratePrecision, decimal sampleRate, bool enabled)
    {
        return new NewCurrencyPairInfo(new CurrencyPairDto(symbol, counterParty, ratePrecision, pipsPosition), sampleRate, enabled);
    }

    public IEnumerable<CurrencyPairInfo> GetAllCurrencyPairs()
    {
        return _currencyPairs.Values;
    }

    public decimal GetSampleRate(string symbol, string counterParty)
    {
        return _currencyPairs[$"{symbol}.{counterParty}"].SampleRate;
    }

    public bool Exists(string symbol, string counterParty)
    {
        return _currencyPairs.ContainsKey($"{symbol}.{counterParty}");
    }
}