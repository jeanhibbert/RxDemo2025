namespace RxAspireApp.Domain.Strategy.Services;

[Serializable]
internal class QuoteException : Exception
{
    public QuoteException()
    {
    }

    public QuoteException(string? message) : base(message)
    {
    }

    public QuoteException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}