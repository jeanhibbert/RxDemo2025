using System.Reactive.Linq;

namespace RxDemo.ConsoleApp;
internal class PipelineSample
{
    public void RunReactivePipeline()
    {
        // Create an observable sequence
        var numbers = Observable.Range(1, 10);

        // Define a pipeline with filtering and transformation
        var pipeline = numbers
            .Where(n => n % 2 == 0) // Filter: Only even numbers
            .Select(n => n * n);   // Transform: Square each number

        // Subscribe to the pipeline and print the results
        pipeline.Subscribe(
            onNext: n => Console.WriteLine($"Processed value: {n}"),
            onError: ex => Console.WriteLine($"Error: {ex.Message}"),
            onCompleted: () => Console.WriteLine("Pipeline completed.")
        );
    }
}
