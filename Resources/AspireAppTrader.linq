<Query Kind="Program">
  <Reference Relative="..\GIT_REPOSITORIES\RxDemo2025\RxAspireApp.Client.Domain\bin\Debug\net8.0\RxAspireApp.Client.Domain.dll">C:\IT_DEVCODE\GIT_REPOSITORIES\RxTalk2025\RxAspireApp.Client.Domain\bin\Debug\net8.0\RxAspireApp.Client.Domain.dll</Reference>
  <Reference Relative="..\GIT_REPOSITORIES\RxDemo2025\RxAspireApp.Client.Domain\bin\Debug\net8.0\RxDemo.Common.dll">C:\IT_DEVCODE\GIT_REPOSITORIES\RxTalk2025\RxAspireApp.Client.Domain\bin\Debug\net8.0\RxDemo.Common.dll</Reference>
  <Namespace>RxAspireApp.Client.Domain.ServiceClients</Namespace>
  <RuntimeVersion>8.0</RuntimeVersion>
</Query>

void Main()
{
IAspireAppTrader aspireAppTrader = new AspireAppTrader("https://localhost:7376/pricinghub");

var jpmStream = aspireAppTrader.PricingServiceClient.GetSpotStream("EURUSD", "JPM");

var bamlStream = aspireAppTrader.PricingServiceClient.GetSpotStream("EURUSD", "BAML");

aspireAppTrader.ConnectionStatusStream.Dump("Connection").Subscribe(conn => conn.Dump());


var stream = jpmStream.Merge(bamlStream);

stream.Select((p, i) => "price" + i + ":" + p.ToString()).Subscribe(sdf => Console.WriteLine(sdf.ToString()));

//
//	var targetPrice = 1.3625m;
//var execution = from price in eurusd.Where(price => price.Bid.Rate < targetPrice).Take(1)
//					from trade in price.Bid.ExecuteRequest(10000, "EUR")
//					select trade.ToString();
//
	var subscription = stream
		.Where(x => x.Bid > 0)
		.Subscribe(x => x.Dump());
	
	
	Task.Delay(20000);
	
	subscription.Dispose();
}

