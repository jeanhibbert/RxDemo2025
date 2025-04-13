namespace RxDemo.Common.Strategy.DTO;
public enum BuySell
{
    Buy,
    Sell
}

public interface IStrategyDetails
{
    string Ticker { get; set; }
    BuySell Instruction { get; set; }
    decimal PriceMovement { get; set; }
    int Quantity { get; set; }
}


public class StrategyDetailsDto : IStrategyDetails
{
    public string Ticker { get; set; }
    public BuySell Instruction { get; set; }
    public decimal PriceMovement { get; set; }
    public int Quantity { get; set; }
}