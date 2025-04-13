namespace RxDemo.Common.Pricing.Extensions
{
    public interface IHeartbeat<out T>
    {
        bool IsHeartbeat { get; }
        T Update { get; }
    }
}