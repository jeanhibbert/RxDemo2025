namespace RxDemo.Common.Pricing;

public static class ServiceConstants
{
    public static class Server
    {
        public const string UsernameHeader = "User";

        public const string PricingHub = "PricingHub";
        public const string SubscribePriceStream = "SubscribePriceStream";
        public const string UnsubscribePriceStream = "UnsubscribePriceStream";
    }

    public static class Client
    {
        public const string OnNewPrice = "OnNewPrice";
        public const string OnNewTrade = "OnNewTrade";
        public const string OnCurrencyPairUpdate = "OnCurrencyPairUpdate";
    }
}
