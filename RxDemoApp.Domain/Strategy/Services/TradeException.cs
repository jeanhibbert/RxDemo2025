namespace RxAspireApp.Domain.Strategy.Services;

[Serializable]
internal class TradeException : Exception
{
    public TradeException()
    {
    }

    public TradeException(string? message) : base(message)
    {
    }

    public TradeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}