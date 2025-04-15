namespace RxDemo.Common.Pricing.DTO;

public record PriceSubscriptionRequestDto
{
    public string CurrencyPair { get; init; }

    public string CounterParty { get; init; }

    public override string ToString()
    {
        return string.Format("CurrencyPair: {0}, CounterParty", CurrencyPair, CounterParty);
    }
}