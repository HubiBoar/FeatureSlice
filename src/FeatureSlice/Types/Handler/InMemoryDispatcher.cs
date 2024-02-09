using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.FeatureManagement;

namespace FeatureSlice;

public static class InMemoryDispatcher<TRequest, TResponse>
{
    public delegate Task<OneOf<TResponse, Error>> Handle(TRequest request);

    public static class Default
    {
        public static Task<OneOf<TResponse, Error>> Dispatch(
            TRequest request,
            IServiceProvider provider,
            Handle handler)
        {
            return Dispatch(
                request,
                handler,
                provider.GetServices<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>>().ToList());
        }

        public static async Task<OneOf<TResponse, Error>> Dispatch(
            TRequest request,
            Handle handler,
            IReadOnlyList<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>> pipelines)
        {
            return await pipelines.RunPipeline(request, r => handler(r));
        }
    }

    public static class WithFlag
    {
        public static Task<OneOf<TResponse, Disabled, Error>> Dispatch(
            TRequest request,
            IServiceProvider provider,
            Handle handler,
            string featureName)
        {
            return Dispatch(
                request,
                handler,
                provider.GetRequiredService<IFeatureManager>(),
                featureName,
                provider.GetServices<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>>().ToList());
        }

        public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch(
            TRequest request,
            Handle handler,
            IFeatureManager featureManager,
            string featureName,
            IReadOnlyList<IPipeline<TRequest, Task<OneOf<TResponse, Error>>>> pipelines)
        {
            var isEnabled = await featureManager.IsEnabledAsync(featureName);
            if(isEnabled == false)
            {
                return new Disabled();
            }

            var result = await InMemoryDispatcher<TRequest, TResponse>.Default.Dispatch(request, handler, pipelines);
            return result.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
        }
    }
}