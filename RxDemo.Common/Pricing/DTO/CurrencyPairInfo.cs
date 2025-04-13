using System.Diagnostics;

namespace RxDemo.Common.Pricing.DTO;

public abstract class CurrencyPairInfo
{
    public CurrencyPairDto CurrencyPair { get; private set; }
    public decimal SampleRate { get; private set; }
    public bool Enabled { get; set; }
    public string Comment { get; set; }
    public bool Stale { get; set; }

    protected CurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled, string comment)
    {
        CurrencyPair = currencyPair;
        SampleRate = sampleRate;
        Enabled = enabled;
        Comment = comment;
    }

    public abstract PriceDto GenerateNextQuote(PriceDto lastPrice);
}

public class CurrencyPairDto
{
    public CurrencyPairDto(string symbol, int ratePrecision, int pipsPosition)
    {
        Symbol = symbol;
        RatePrecision = ratePrecision;
        PipsPosition = pipsPosition;
    }

    public string Symbol { get; private set; }
    public int RatePrecision { get; private set; }
    public int PipsPosition { get; private set; }

    public override string ToString()
    {
        return string.Format("Symbol: {0}, RatePrecision: {1}, PipsPosition: {2}", Symbol, RatePrecision, PipsPosition);
    }
}

public sealed class CyclicalCurrencyPairInfo : CurrencyPairInfo
{
    private readonly decimal _min;
    private readonly decimal _max;
    private bool _isIncreasing = false;

    public CyclicalCurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled, string comment, decimal min, decimal max)
        : base(currencyPair, sampleRate, enabled, comment)
    {
        _min = min;
        _max = max;
    }

    public override PriceDto GenerateNextQuote(PriceDto previousPrice)
    {
        var pow = (decimal)Math.Pow(10, CurrencyPair.RatePrecision);
        var movement = 5 / pow;

        decimal newMid;
        if (_isIncreasing)
        {
            newMid = previousPrice.Mid + movement;
            if (newMid > _max)
                _isIncreasing = false;
        }
        else
        {
            newMid = previousPrice.Mid - movement;
            if (newMid < _min)
                _isIncreasing = true;
        }

        return new PriceDto
        {
            Symbol = previousPrice.Symbol,
            QuoteId = previousPrice.QuoteId + 1,
            CreatedDate = DateTime.UtcNow,
            Mid = newMid,
            Ask = newMid + 5 / pow,
            Bid = newMid - 5 / pow,
            CreationTimestamp = Stopwatch.GetTimestamp()
        };
    }
}

public sealed class RandomWalkCurrencyPairInfo : CurrencyPairInfo
{
    private static readonly Random _random = new Random();

    public RandomWalkCurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled, string comment)
        : base(currencyPair, sampleRate, enabled, comment)
    {
    }

    public override PriceDto GenerateNextQuote(PriceDto previousPrice)
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
            CreationTimestamp = Stopwatch.GetTimestamp()
        };
    }
}