using System.Reactive.Linq;

namespace RxDemo.ConsoleApp;
public static class EventHandlersVsObservableStreams
{
    public static void Sample1()
    {
        // Demo 1 Create an eventhandler and evoke it

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


        //var stream = Observable.FromEventPattern<int>(
        //  handler => eventHandler += handler,
        //  handler => eventHandler -= handler).Select(x => x.EventArgs);

        //var subscription = stream.Where(x => x % 2 == 0) // Filter the events
        //    .Subscribe(x => Console.WriteLine(x));

        //eventHandler -= HandleEvent; // would create a null reference exception

        // Invoke the event
        eventHandler.Invoke(null, 1);
        eventHandler.Invoke(null, 2);
        eventHandler.Invoke(null, 3);
        eventHandler.Invoke(null, 4);

        //eventHandler -= HandleEvent;

        // Unsubscribe from the event




        // Invoke again to see the effect of unsubscription

    }
}
