using RxDemo.Common.Pricing.Extensions;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxAspireApp.Client.Domain.Transport;

public interface IConnectionProvider
{
    IObservable<IConnection> GetActiveConnection();
}

/// <summary>
/// Connection provider provides always the same connection until it fails then create a new one a yield it
/// </summary>
public class ConnectionProvider : IConnectionProvider, IDisposable
{
    private readonly SingleAssignmentDisposable _disposable = new SingleAssignmentDisposable();
    private readonly string _username;
    private readonly IObservable<IConnection> _connectionSequence;
    private readonly string _serverUrl;

    public ConnectionProvider(string username, string serverUrl)
    {
        _username = username;
        _serverUrl = serverUrl;

        _connectionSequence = CreateConnectionSequence();
    }

    public IObservable<IConnection> GetActiveConnection()
    {
        return _connectionSequence;
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    private IObservable<IConnection> CreateConnectionSequence()
    {
        return Observable.Create<IConnection>(o =>
        {
            Debug.WriteLine("Creating new connection...");
            var connection = new Connection(_serverUrl, _username);

            var statusSubscription = connection.StatusStream.Subscribe(
                _ => { },
                ex => o.OnCompleted(),
                () =>
                {
                    Debug.WriteLine("Status subscription completed");
                    o.OnCompleted();
                });

            var connectionSubscription =
                connection.Initialize().Subscribe(
                    _ => o.OnNext(connection),
                    ex => o.OnCompleted(),
                    o.OnCompleted);

            return new CompositeDisposable { statusSubscription, connectionSubscription };
        })
            .Repeat()
            .Replay(1)
            .LazilyConnect(_disposable);
    }
}