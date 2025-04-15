namespace RxDemo.Common.Pricing.DTO;

public record PriceDto
{
    public string Symbol { get; init; }
    public long QuoteId { get; init; }
    public decimal Bid { get; init; }
    public decimal Ask { get; init; }
    public DateTime CreatedDate { get; init; }
    public decimal Mid { get; init; }
    public long CreationTimestamp { get; init; }
    public string CounterParty { get; init; }

    public override string ToString()
    {
        return string.Format("Symbol: {0}, Counterparty: {1}, QuoteId: {2}, Bid: {3}, Ask: {4}, CreatedDate: {5}", Symbol, CounterParty, QuoteId, Bid, Ask, CreatedDate);
    }
}
