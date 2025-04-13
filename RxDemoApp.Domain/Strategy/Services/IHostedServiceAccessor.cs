using Microsoft.Extensions.Hosting;

namespace RxAspireApp.Domain.Strategy.Services;

public interface IHostedServiceAccessor<out T> where T : IHostedService
{
    T Service { get; }
}