namespace RxDemo.Common.Pricing.Extensions
{
    class Stale<T> : IStale<T>
    {
        public Stale() : this(true, default)
        {
        }

        public Stale(T update) : this(false, update)
        {
        }

        private Stale(bool isStale, T update)
        {
            IsStale = isStale;
            Update = update;
        }

        public bool IsStale { get; private set; }
        public T Update { get; private set; }
    }
}