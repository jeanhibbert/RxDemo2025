using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

internal class PipelineSample
{
    private readonly ISubject<bool> connectionStream = new BehaviorSubject<bool>(false);

    public void RunReactivePipeline()
    {
        var reportPublisher = new ReportPublisher();
        var statsFeedPublisher = new StatsFeedPublisher();

        var externalDataStream = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => new Data(true, Random.Shared.Next(1, 5)))
            .Publish()
            .RefCount();

        var reportStream = connectionStream
            .Where(connectionSuccess => connectionSuccess)
            .SelectMany(_ => externalDataStream
                .Where(data => data.IsValid)
                .DistinctUntilChanged()
                .ObserveOn(TaskPoolScheduler.Default)
                .Sample(TimeSpan.FromSeconds(5))
                .SelectMany(data => Observable.FromAsync(() => statsFeedPublisher.PublishAsync(data)))
                .Where(statsFeed => statsFeed.IsSuccessfulPublish)
                .SelectMany(statsFeed => Observable.FromAsync(() => reportPublisher.BuildReportsAsync(statsFeed)))
                .Where(report => report.IsSuccessfulBuild)
                .Select(ProcessReport));

        // Subscribe to the pipeline to execute it and see output
        reportStream.Subscribe(
            report => Console.WriteLine($"Processed Report: {report.ReportData}, Processed: {report.IsProcessed}"),
            error => Console.WriteLine($"Error: {error.Message}"),
            () => Console.WriteLine("Pipeline completed")
        );
    }

    public void Connect()
    {
        connectionStream.OnNext(true); // Simulate a successful connection
    }

    private Report ProcessReport(Report report)
    {
        return report with { IsProcessed = true };
    }
}

public record Data(bool IsValid, int Number);

public class StatsFeedPublisher
{
    public async Task<StatsFeed> PublishAsync(Data data)
    {
        Console.WriteLine($"Publishing {data} messages in {TimeSpan.FromSeconds(1)} seconds.");
        await Task.Delay(TimeSpan.FromSeconds(1));
        return new StatsFeed("Published", true);
    }
}

public record StatsFeed(string Message, bool IsSuccessfulPublish);

public class ReportPublisher
{
    public async Task<Report> BuildReportsAsync(StatsFeed statsFeed)
    {
        Console.WriteLine($"Building {statsFeed} reports in {TimeSpan.FromSeconds(1)} seconds.");
        await Task.Delay(TimeSpan.FromSeconds(1));
        return new Report(Guid.NewGuid().ToString(), true);
    }
}

public record Report(string ReportData, bool IsSuccessfulBuild)
{
    public bool IsProcessed { get; init; }
}

