using System.Runtime.Serialization;

namespace RxAspireApp.Client.Domain.Transport;
[Serializable]
public class TransportDisconnectedException : Exception
{
    public TransportDisconnectedException()
    {
    }

    public TransportDisconnectedException(string message) : base(message)
    {
    }

    public TransportDisconnectedException(string message, Exception inner) : base(message, inner)
    {
    }

    protected TransportDisconnectedException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}