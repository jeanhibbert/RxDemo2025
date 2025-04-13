using System.Reactive.Linq;

namespace RxDemo.ConsoleApp;
public class BasicSamples
{
    public void CreateStreamsAndMergeCatch()
    {
        //1) Create a basic observable sequence of characters
        var stream1 = "hello world".ToObservable().Select(x =>
        {
            //if (x == 'r')
            //throw new InvalidDataException("Invalid data!!");

            return x.ToString();
        });

        //var subscription = stream1.Subscribe(
        //    x => Console.WriteLine(x),
        //    ex => Console.WriteLine(ex.Message),
        //    () => Console.WriteLine("Completed"))


        //2) Create an observable sequence of integers
        var stream2 = Observable.Range(1, 10)
            .Select(x =>
            {
                if (x == 8)
                    throw new InvalidDataException("Invalid data!!");
                return x.ToString();
            });

        //var subscription2 = stream2.Where(x => x % 2 == 0).Subscribe(x => Console.WriteLine(x),
        //    ex => Console.WriteLine(ex.Message),
        //    () => Console.WriteLine("Completed"));

        var stream3 = stream2.Merge(stream1)
            .Select(x =>
            {
                return x;
            });

        stream3.Subscribe(x => Console.WriteLine(x),
            ex => Console.WriteLine($"what the heck happened? {ex.Message}"),
            () => Console.WriteLine("Completed"));
    }

    public IObservable<string> Sample2()
    {
        //2) Create an observable sequence of integers
        var stream2 = Observable.Range(1, 10)
            .Select(x =>
            {
                if (x == 8)
                    throw new InvalidDataException("Invalid data!!");
                return x.ToString();
            });

        //var subscription2 = stream2.Where(x => x % 2 == 0).Subscribe(x => Console.WriteLine(x),
        //    ex => Console.WriteLine(ex.Message),
        //    () => Console.WriteLine("Completed"));

        return stream2;
    }

}
