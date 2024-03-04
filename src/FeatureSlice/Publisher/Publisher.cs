using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Else;
using OneOf.Types;

namespace FeatureSlice;

public sealed class Publisher : IPublisher
{
    private readonly IServiceProvider _provider;

    public Publisher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task<OneOf<Success, Error>> Dispatch<TRequest>(TRequest request)
    {
        using var scope = _provider.CreateScope();

        var provider = scope.ServiceProvider;

        return Dispatch<TRequest>(request, provider.GetServices<IPublisher.Listen<TRequest>>().ToArray());
    }

    public Task<OneOf<Success, Error>> DispatchParallel<TRequest>(TRequest request)
    {
        using var scope = _provider.CreateScope();

        var provider = scope.ServiceProvider;

        return DispatchParallel<TRequest>(request, provider.GetServices<IPublisher.Listen<TRequest>>().ToArray());
    }
    
    public static async Task<OneOf<Success, Error>> Dispatch<TRequest>(TRequest request, IReadOnlyCollection<IPublisher.Listen<TRequest>> listeners)
    {
        foreach(var listener in listeners)
        {
            var result = await listener(request);

            if(result.Is(out Error error))
            {
                return error;
            }
        }

        return new Success();
    }

    public static async Task<OneOf<Success, Error>> DispatchParallel<TRequest>(TRequest request, IReadOnlyCollection<IPublisher.Listen<TRequest>> listeners)
    {
        var tasks = listeners.Select(listener => listener(request));

        var results = await Task.WhenAll(tasks);

        if(results.Any(x => x.IsT1))
        {
            //TODO Combine errors as result
            return new Error();
        }

        return new Success();
    }
}

public interface IPublisher
{
    public Task<OneOf<Success, Error>> Dispatch<TRequest>(TRequest request);
    public Task<OneOf<Success, Error>> DispatchParallel<TRequest>(TRequest request);

    public delegate Task<OneOf<Success, Error>> Listen<TRequest>(TRequest request);

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IPublisher, Publisher>();
    }

    public static void RegisterListener<TRequest>(IServiceCollection services, ServiceFactory<Listen<TRequest>> factory)
    {
        services.AddSingleton(factory);
    }
}