using System.Reactive.Subjects;

namespace RxDemo.ConsoleApp;
public static class SubjectSamples
{
    public static void BasicSubject()
    {
        var stream = new Subject<int>();

        // Subscribe two observers
        var subscription1 = stream.Subscribe(value => Console.WriteLine($"Observer 1: {value}"));
        var subscription2 = stream.Subscribe(value => Console.WriteLine($"Observer 2: {value}"));

        // Push values into the subject
        stream.OnNext(1); // Outputs: Observer 1: 1, Observer 2: 1
        stream.OnNext(2); // Outputs: Observer 1: 2, Observer 2: 2
        stream.OnCompleted(); // Completes both observers

        // Dispose of the subscriptions
        subscription1.Dispose();
        subscription2.Dispose();
    }

    public static void BehaviorSubject()
    {
        var behaviorSubject = new BehaviorSubject<int>(0); // Initial value is 0
        // Subscribe to the BehaviorSubject
        var subscription = behaviorSubject.Subscribe(value => Console.WriteLine($"Observer: {value}"));
        // Push values into the BehaviorSubject
        behaviorSubject.OnNext(1); // Outputs: Observer: 1
        behaviorSubject.OnNext(2); // Outputs: Observer: 2
        // Dispose of the subscription
        subscription.Dispose();
    }

    public static void ReplaySubject()
    {
        var replaySubject = new ReplaySubject<int>(2); // Buffer size is 2
        // Subscribe to the ReplaySubject
        var subscription = replaySubject.Subscribe(value => Console.WriteLine($"Observer: {value}"));
        // Push values into the ReplaySubject
        replaySubject.OnNext(1); // Outputs: Observer: 1
        replaySubject.OnNext(2); // Outputs: Observer: 2
        replaySubject.OnNext(3); // Outputs: Observer: 3
        // Dispose of the subscription
        subscription.Dispose();
    }

    public static void AsyncSubject()
    {
        var asyncSubject = new AsyncSubject<int>();
        // Subscribe to the AsyncSubject
        var subscription = asyncSubject.Subscribe(value => Console.WriteLine($"Observer: {value}"));
        // Push values into the AsyncSubject
        asyncSubject.OnNext(1); // No output yet
        asyncSubject.OnNext(2); // No output yet
        asyncSubject.OnCompleted(); // Outputs: Observer: 2 (last value)
        // Dispose of the subscription
        subscription.Dispose();
    }

    public static void Run()
    {
        Console.WriteLine("Basic Subject:");
        BasicSubject();
        Console.WriteLine("\nBehavior Subject:");
        BehaviorSubject();
        Console.WriteLine("\nReplay Subject:");
        ReplaySubject();
        Console.WriteLine("\nAsync Subject:");
        AsyncSubject();
    }

    public static void MessagePublisherSample()
    {
        // Create an instance of MessagePublisher
        var publisher = new MessagePublisher();

        // Subscriber 1: Prints messages with a prefix
        publisher.MessageStream.Subscribe(
            message => Console.WriteLine($"Subscriber 1: {message}"),
                    () => Console.WriteLine("Subscriber 1: Stream completed.")
                );

        // Subscriber 2: Prints messages in uppercase
        publisher.MessageStream.Subscribe(
            message => Console.WriteLine($"Subscriber 2: {message.ToUpper()}"),
            () => Console.WriteLine("Subscriber 2: Stream completed.")
        );

        // Start publishing messages
        Console.WriteLine("Publishing messages...");
        publisher.PublishMessages();
    }


}

public class MessagePublisher
{
    private readonly Subject<string> _messageStream = new Subject<string>();

    // Public IObservable endpoint for subscribers
    public IObservable<string> MessageStream => _messageStream;

    // Method to publish messages directly
    public void PublishMessages()
    {
        _messageStream.OnNext("Message 1: Hello, subscribers!");
        _messageStream.OnNext("Message 2: This is a reactive stream.");
        _messageStream.OnNext("Message 3: Final message.");
        _messageStream.OnCompleted();
    }
}