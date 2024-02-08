using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.FeatureManagement;

namespace FeatureSlice;

public static class InMemoryDispatcher
{
    public static Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        IServiceProvider provider)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return Dispatch(
            request,
            provider.GetRequiredService<THandler>(),
            provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
    }

    public static async Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        THandler self,
        IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return await pipelines.RunPipeline(request, self.Handle);
    }

    public static class WithFlag
    {
        public static Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
            TRequest request,
            IServiceProvider provider,
            string featureName)
            where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {
            return Dispatch(
                request,
                provider.GetRequiredService<THandler>(),
                provider.GetRequiredService<IFeatureManager>(),
                featureName,
                provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
        }

        public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
            TRequest request,
            THandler self,
            IFeatureManager featureManager,
            string featureName,
            IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
            where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {
            var isEnabled = await featureManager.IsEnabledAsync(featureName);
            if(isEnabled == false)
            {
                return new Disabled();
            }

            var result = await InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, self, pipelines);
            return result.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
        }
    }
}