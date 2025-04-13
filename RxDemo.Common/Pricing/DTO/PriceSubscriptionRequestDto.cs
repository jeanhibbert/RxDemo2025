namespace RxDemo.Common.Pricing.DTO;

public record PriceSubscriptionRequestDto
{
    public string CurrencyPair { get; set; }

    public override string ToString()
    {
        return string.Format("CurrencyPair: {0}", CurrencyPair);
    }
}