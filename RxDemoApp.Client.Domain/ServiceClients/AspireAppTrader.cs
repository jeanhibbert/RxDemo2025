using RxAspireApp.Client.Domain.Transport;
using System.Diagnostics;
using System.Reactive.Linq;

namespace RxAspireApp.Client.Domain.ServiceClients;
public interface IAspireAppTrader
{
    IPricingServiceClient PricingServiceClient { get; set; }
    IObservable<ConnectionInfo> ConnectionStatusStream { get; }
    void Dispose();
}

public class AspireAppTrader : IAspireAppTrader
{
    private ConnectionProvider _connectionProvider;
    
    public IPricingServiceClient PricingServiceClient { get; set; }

    public AspireAppTrader(string pricingHubUrl)
    {
        _connectionProvider = new ConnectionProvider("username", pricingHubUrl);
        PricingServiceClient = new PricingServiceClient(_connectionProvider);
    }

    public IObservable<ConnectionInfo> ConnectionStatusStream
    {
        get
        {
            return _connectionProvider.GetActiveConnection()
                .Do(_ => Debug.WriteLine("New connection created by connection provider"))
                .Select(c => c.StatusStream)
                .Switch()
                .Publish()
                .RefCount();
        }
    }

    public void Dispose()
    {
        _connectionProvider.Dispose();
    }
}
