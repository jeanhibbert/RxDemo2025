using Microsoft.AspNetCore.SignalR.Client;
using RxDemo.Common.Pricing.Extensions;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxAspireApp.Client.Domain.Transport;

public interface IConnection
{
    IObservable<ConnectionInfo> StatusStream { get; }
    IObservable<Unit> Initialize();
    string Address { get; }
    HubConnection PricingHubConnection { get; }
}

public class Connection : IConnection
{
    private readonly ISubject<ConnectionInfo> _statusStream;
    private readonly HubConnection _hubConnection;

    private bool _initialized;

    public Connection(string address, string username)
    {
        _statusStream = new BehaviorSubject<ConnectionInfo>(new ConnectionInfo(ConnectionStatus.Uninitialized, address));

        Address = address;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(address)
            .WithAutomaticReconnect()
            .Build();
        
        CreateStatus().Subscribe(
            s => _statusStream.OnNext(new ConnectionInfo(s, address)),
            _statusStream.OnError,
            _statusStream.OnCompleted);

        _hubConnection.Closed += _hubConnection_Closed;

        PricingHubConnection = _hubConnection;
    }

    private async Task _hubConnection_Closed(Exception? arg)
    {
        Debug.WriteLine("Connection closed " + arg.Message);
    }

    public IObservable<Unit> Initialize()
    {
        if (_initialized)
        {
            throw new InvalidOperationException("Connection has already been initialized");
        }
        _initialized = true;

        return Observable.Create<Unit>(async observer =>
        {
            _statusStream.OnNext(new ConnectionInfo(ConnectionStatus.Connecting, Address)); 

            try
            {
                Debug.WriteLine($"Connecting to {Address}");
                await _hubConnection.StartAsync();
                _statusStream.OnNext(new ConnectionInfo(ConnectionStatus.Connected, Address));
                observer.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured when starting SignalR connection", e);
                observer.OnError(e);
            }

            return Disposable.Create(() =>
            {
                try
                {
                    Debug.WriteLine("Stoping connection...");
                    _hubConnection.StopAsync();
                    Debug.WriteLine("Connection stopped");
                }
                catch (Exception e)
                {
                    // we must never throw in a disposable
                    Debug.WriteLine("An error occured while stoping connection", e);
                }
            });
        })
        .Publish()
        .RefCount();
    } 

  private IObservable<ConnectionStatus> CreateStatus()
    {
        var closed = Observable.FromEvent<Func<Exception?, Task>, Exception?>(
            handler => async (exception) => handler(exception),
            handler => _hubConnection.Closed += handler,
            handler => _hubConnection.Closed -= handler
        ).Select(_ => ConnectionStatus.Closed);

        var reconnected = Observable.FromEvent<Func<string?, Task>, string?>(
            handler => async (message) => handler(message),
            handler => _hubConnection.Reconnected += handler,
            handler => _hubConnection.Reconnected -= handler
        ).Select(_ => ConnectionStatus.Reconnected);

        var reconnecting = Observable.FromEvent<Func<Exception?, Task>, Exception?>(
            handler => async (exception) => handler(exception),
            handler => _hubConnection.Reconnecting += handler,
            handler => _hubConnection.Reconnecting -= handler
        ).Select(_ => ConnectionStatus.Reconnecting);

        return Observable.Merge(closed, reconnected, reconnecting)
            .TakeUntilInclusive(status => status == ConnectionStatus.Closed);
    }

    public IObservable<ConnectionInfo> StatusStream
    {
        get { return _statusStream; }
    }

    public string Address { get; private set; }

    public HubConnection PricingHubConnection { get; private set; }
    
    public override string ToString()
    {
        return string.Format("Address: {0}", Address);
    }
}