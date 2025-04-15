using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxDemo.ConsoleApp;
internal class PublishRefCountSample
{
    public class Sample
    {
        // REFINE
        public static void Run()
        {
            //IConnectableObservable<long> publishedTicks = Observable
            //    .Interval(TimeSpan.FromSeconds(1))
            //    .Take(4)
            //    .Publish();

            IObservable<long> publishedTicks = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Take(4);
                

            publishedTicks.Subscribe(x => Console.WriteLine($"Sub1: {x} ({DateTime.Now})"));
            publishedTicks.Subscribe(x => Console.WriteLine($"Sub2: {x} ({DateTime.Now})"));

            //publishedTicks.Connect();
            Thread.Sleep(2500);
            Console.WriteLine();
            Console.WriteLine("Adding more subscribers");
            Console.WriteLine();

            publishedTicks.Subscribe(x => Console.WriteLine($"Sub3: {x} ({DateTime.Now})"));
            publishedTicks.Subscribe(x => Console.WriteLine($"Sub4: {x} ({DateTime.Now})"));
        }
    }
}
