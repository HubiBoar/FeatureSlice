using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Else;
using OneOf.Types;

namespace FeatureSlice;

public static class Publisher<TRequest>
{
    public delegate Task<OneOf<Success, Error>> Dispatch(TRequest request);

    public delegate Task<OneOf<Success, Error>> DispatchParallel(TRequest request);

    public delegate Task<OneOf<Success, Error>> Listen(TRequest request);

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton(RegisterDispatcher);
        services.AddSingleton(RegisterDispatcherParallel);

        static Dispatch RegisterDispatcher(IServiceProvider provider)
        {
            return request => Dispatcher(
                request,
                provider.GetServices<Listen>().ToList());
        }

        static DispatchParallel RegisterDispatcherParallel(IServiceProvider provider)
        {
            return request => DispatcherParallel(
                request,
                provider.GetServices<Listen>().ToList());
        }
    }

    public static async Task<OneOf<Success, Error>> Dispatcher(TRequest request, IReadOnlyCollection<Listen> listeners)
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

    public static async Task<OneOf<Success, Error>> DispatcherParallel(TRequest request, IReadOnlyCollection<Listen> listeners)
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