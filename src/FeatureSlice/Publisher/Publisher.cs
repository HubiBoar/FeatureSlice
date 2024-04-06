using Microsoft.Extensions.DependencyInjection;using Definit.Results;

namespace FeatureSlice;

public sealed class Publisher : IPublisher
{
    private readonly IServiceProvider _provider;

    public Publisher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task<Result> Dispatch<TRequest>(TRequest request)
    {
        using var scope = _provider.CreateScope();

        var provider = scope.ServiceProvider;

        return Dispatch(request, provider.GetServices<IPublisher.Listen<TRequest>>().ToArray());
    }

    public Task<Result> DispatchParallel<TRequest>(TRequest request)
    {
        using var scope = _provider.CreateScope();

        var provider = scope.ServiceProvider;

        return DispatchParallel(request, provider.GetServices<IPublisher.Listen<TRequest>>().ToArray());
    }
    
    public static async Task<Result> Dispatch<TRequest>(TRequest request, IReadOnlyCollection<IPublisher.Listen<TRequest>> listeners)
    {
        foreach(var listener in listeners)
        {
            var result = await listener(request);

            if(result.Is(out Error error))
            {
                return error;
            }
        }

        return Result.Success;
    }

    public static async Task<Result> DispatchParallel<TRequest>(TRequest request, IReadOnlyCollection<IPublisher.Listen<TRequest>> listeners)
    {
        var tasks = listeners.Select(listener => listener(request));

        var results = await Task.WhenAll(tasks);

        var errors = results.SelectWhere(x => (x.Is(out Error error), error)).ToArray();
        
        if (errors.Length == 1)
        {
            return errors.Single();
        }

        if (errors.Length > 1)
        {
            return new Error(string.Join(", ", errors.Select(x => x.Message)));
        }

        return Result.Success;
    }
}

public interface IPublisher
{
    public Task<Result> Dispatch<TRequest>(TRequest request);
    public Task<Result> DispatchParallel<TRequest>(TRequest request);

    public delegate Task<Result> Listen<TRequest>(TRequest request);

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IPublisher, Publisher>();
    }

    public static void RegisterListener<TRequest>(IServiceCollection services, ServiceFactory<Listen<TRequest>> factory)
    {
        services.AddSingleton(factory);
    }
}