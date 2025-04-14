using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxDemo.ConsoleApp;
public static class EventHandlersVsObservableStreams
{
    public static void Sample2()
    {

        var subscription1 = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(x => Console.WriteLine(x));


        Console.ReadLine();


        ISubject<int> stream = new BehaviorSubject<int>(20);

        ISubject<int> stream2 = new ReplaySubject<int>();

        stream2.OnNext(1);
        stream2.OnNext(2);
        stream2.OnNext(2);
        stream2.OnNext(2);
        stream2.OnNext(3);
        stream2.OnNext(4);

        var subscription = stream
            .Merge(stream2)
            .Where(x => x % 2 == 0)
            .DistinctUntilChanged()
            .Subscribe(x => Console.WriteLine(x));

        stream.OnCompleted();

        Console.ReadLine();

        subscription.Dispose();

    }

    public static void Sample1()
    {
        //unsubscribe from event handler
        // create an observable sequence
        BasicSamples samples = new BasicSamples();

        EventHandler<int> eventHandler = null;
        eventHandler += HandleEvent;

        void HandleEvent(object? sender, int e)
        {
            if (e % 2 == 0) // logic on when to handle event
                Console.WriteLine($"Event triggered with value: {e}");
        }

        //eventHandler -= HandleEvent; // would create a null reference exception

        // Invoke the event
        eventHandler.Invoke(null, 1);
        eventHandler.Invoke(null, 2);
        eventHandler.Invoke(null, 3);
        eventHandler.Invoke(null, 4);

        // Unsubscribe from the event
        eventHandler -= HandleEvent;

        




        // Invoke again to see the effect of unsubscription

        //var stream = Observable.FromEventPattern<int>(
        //  handler => eventHandler += handler,
        //  handler => eventHandler -= handler).Select(x => x.EventArgs);

        //var subscription = stream.Where(x => x % 2 == 0) // Filter the events
        //    .Subscribe(x => Console.WriteLine(x));

    }
}
