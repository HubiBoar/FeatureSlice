using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.FeatureManagement;

namespace FeatureSlice;

public static class InMemoryDispatcher
{
    public delegate Task<OneOf<TResponse, Error>> Handle<TRequest, TResponse>(TRequest request);

    public static Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse>(
        TRequest request,
        IServiceProvider provider,
        Handle<TRequest, TResponse> handler)
    {
        return Dispatch(
            request,
            handler,
            provider.GetServices<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>>().ToList());
    }

    public static async Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse>(
        TRequest request,
        Handle<TRequest, TResponse> handler,
        IReadOnlyList<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>> pipelines)
    {
        return await pipelines.RunPipeline(request, r => handler(r));
    }

    public static class WithFlag
    {
        public static Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse>(
            TRequest request,
            IServiceProvider provider,
            Handle<TRequest, TResponse> handler,
            string featureName)
        {
            return Dispatch(
                request,
                handler,
                provider.GetRequiredService<IFeatureManager>(),
                featureName,
                provider.GetServices<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>>().ToList());
        }

        public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse>(
            TRequest request,
            Handle<TRequest, TResponse> handler,
            IFeatureManager featureManager,
            string featureName,
            IReadOnlyList<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>> pipelines)
        {
            var isEnabled = await featureManager.IsEnabledAsync(featureName);
            if(isEnabled == false)
            {
                return new Disabled();
            }

            var result = await InMemoryDispatcher.Dispatch<TRequest, TResponse>(request, handler, pipelines);
            return result.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
        }
    }
}