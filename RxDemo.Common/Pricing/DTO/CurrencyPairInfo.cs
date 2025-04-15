using System.Diagnostics;

namespace RxDemo.Common.Pricing.DTO;

public abstract record CurrencyPairInfo
{
    public CurrencyPairDto CurrencyPair { get; private set; }
    public decimal SampleRate { get; private set; }
    public bool Enabled { get; set; }

    protected CurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled)
    {
        CurrencyPair = currencyPair;
        SampleRate = sampleRate;
        Enabled = enabled;
    }

    public abstract PriceDto NextQuote(PriceDto lastPrice);
}

public record CurrencyPairDto
{
    public CurrencyPairDto(string symbol, string counterParty, int ratePrecision, int pipsPosition)
    {
        Symbol = symbol;
        CounterParty = counterParty;
        RatePrecision = ratePrecision;
        PipsPosition = pipsPosition;
    }

    public string Symbol { get; private set; }
    public string CounterParty { get; private set; }
    public int RatePrecision { get; private set; }
    public int PipsPosition { get; private set; }

    public override string ToString()
    {
        return string.Format("Symbol: {0}, CounterParty{1}, RatePrecision: {2}, PipsPosition: {3}", Symbol, CounterParty, RatePrecision, PipsPosition);
    }
}

public sealed record NewCurrencyPairInfo : CurrencyPairInfo
{
    private static readonly Random _random = new Random();

    public NewCurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled)
        : base(currencyPair, sampleRate, enabled)
    {
    }

    public override PriceDto NextQuote(PriceDto previousPrice)
    {
        var pow = (decimal)Math.Pow(10, CurrencyPair.RatePrecision);
        var newMid = previousPrice.Mid + _random.Next(-5, 5) / pow;

        return new PriceDto
        {
            Symbol = previousPrice.Symbol,
            QuoteId = previousPrice.QuoteId + 1,
            CreatedDate = DateTime.UtcNow,
            Mid = newMid,
            Ask = newMid + 5 / pow,
            Bid = newMid - 5 / pow,
            CounterParty = previousPrice.CounterParty,
            CreationTimestamp = Stopwatch.GetTimestamp()
        };
    }
}