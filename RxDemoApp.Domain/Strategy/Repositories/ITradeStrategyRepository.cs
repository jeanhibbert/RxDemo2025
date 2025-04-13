using RxAspireApp.Domain.Strategy.Model;
using RxDemo.Common.Strategy.DTO;
using System.Reactive.Linq;

namespace RxAspireApp.Domain.Strategy.Repositories;

public interface ITradeStrategyRepository
{
    List<StrategyDetailsDto> TradeStrategyDetails { get; }

    List<TradeStrategy> TradeStrategies { get; }

    IList<string> AddTradeStrategy(IStrategyDetails strategyDetails, IObservable<IPrice> startingPrice);

    bool RemoveStrategy(string strategyId);
}

public class TradeStrategyRepository : ITradeStrategyRepository
{
    public List<TradeStrategy> TradeStrategies { get; } = new List<TradeStrategy>();

    public List<StrategyDetailsDto> TradeStrategyDetails =>
        TradeStrategies.Select(x => new StrategyDetailsDto
        {
            Ticker = x.StrategyDetails.Ticker,
            Instruction = x.StrategyDetails.Instruction,
            PriceMovement = x.StrategyDetails.PriceMovement,
            Quantity = x.StrategyDetails.Quantity
        }).ToList();

    public IList<string> AddTradeStrategy(IStrategyDetails strategyDetails, IObservable<IPrice> priceStream)
    {
        lock (TradeStrategies)
        {
            TradeStrategies.Add(new TradeStrategy(strategyDetails, priceStream));
            return TradeStrategies.Select(x => x.TradeStrategyId).ToList();
        }
    }

    public bool RemoveStrategy(string strategyId)
    {
        lock (TradeStrategies)
        {
            var tradeStrategyToRemove = TradeStrategies.SingleOrDefault(x => x.TradeStrategyId == strategyId);
            if (tradeStrategyToRemove != null)
            {
                TradeStrategies.Remove(tradeStrategyToRemove);
                return true;
            }
            return false;
        }
    }
}
