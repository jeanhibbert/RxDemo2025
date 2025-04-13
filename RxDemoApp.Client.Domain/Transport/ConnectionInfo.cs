namespace RxAspireApp.Client.Domain.Transport;

public enum ConnectionStatus
{
    Connecting,
    Connected,
    ConnectionSlow,
    Reconnecting,
    Reconnected,
    Closed,
    Uninitialized
}

public record ConnectionInfo
{
    public ConnectionStatus ConnectionStatus { get; private set; }
    public string Server { get; private set; }

    public ConnectionInfo(ConnectionStatus connectionStatus, string server)
    {
        ConnectionStatus = connectionStatus;
        Server = server;
    }
}
