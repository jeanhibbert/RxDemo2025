namespace RxDemo.Common.Pricing.Extensions
{
    internal class Heartbeat<T> : IHeartbeat<T>
    {
        public bool IsHeartbeat { get; private set; }
        public T Update { get; private set; }

        public Heartbeat() : this(true, default)
        {
        }

        public Heartbeat(T update) : this(false, update)
        {
        }

        private Heartbeat(bool isHeartbeat, T update)
        {
            IsHeartbeat = isHeartbeat;
            Update = update;
        }
    }
}