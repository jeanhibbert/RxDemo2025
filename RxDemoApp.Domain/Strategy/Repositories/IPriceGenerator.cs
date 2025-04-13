using Microsoft.Extensions.Logging;
using RxAspireApp.Domain.Strategy.Factories;
using RxAspireApp.Domain.Strategy.Model;
using RxAspireApp.Domain.Strategy.Services;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxAspireApp.Domain.Strategy.Repositories
{
    public interface IPriceGenerator : IDisposable
    {
        IObservable<IPrice> GetPriceStream(ITickerData tickerData);
    }
         
    public class PriceGenerator : IPriceGenerator
    {

        private IDisposable _subscription = null;
        private ISubject<PriceDto> _priceStream = new ReplaySubject<PriceDto>(1);
        private readonly ILogger<TickerDataFactory> _logger;
        private readonly IBloombergService _bloombergService;
        private readonly int _priceCheckInterval = ServiceConstants.Server.PriceStreamIntervalMilliseconds;

        public PriceGenerator(ILogger<TickerDataFactory> logger, IBloombergService bloombergService)
        {
            _logger = logger;
            _bloombergService = bloombergService;
        }

        public IObservable<IPrice> GetPriceStream(ITickerData tickerData)
        {
            IObservable<long> source = Observable.Interval(TimeSpan.FromMilliseconds(_priceCheckInterval));
            _subscription = source.Subscribe(
                x =>
                {
                    var newPrice = GetChange(tickerData.Ticker);
                    if (newPrice.HasValue)
                    {
                        var price = new PriceDto(tickerData.Ticker, tickerData, newPrice.Value);
                        _priceStream.OnNext(price);
                    }
                },
                ex =>
                {
                    _logger.LogWarning("Error loading price from bloomberg Api: {0}", ex.Message);
                },
                () => _logger.LogWarning("Stopped loading prices from bloomberg Api"));

            return _priceStream;
        }

        private decimal? GetChange(string ticker)
        {
            try
            {
                return _bloombergService.GetQuote(ticker);
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"bloomberg API threw exception : {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            if (_subscription != null)
                _subscription.Dispose();
        }


    }
}
