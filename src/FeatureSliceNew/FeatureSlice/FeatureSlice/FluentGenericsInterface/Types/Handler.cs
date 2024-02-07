using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.FeatureManagement;

namespace FeatureSlice.FluentGenerics.Interfaces;

public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
{
}

public static class HandlerFeatureSlice
{
    public interface Default<TRequest, TResponse, THandler> : DelegateFeatureSlice.Default<TRequest, TResponse>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        protected static void RegisterInternal(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterInternal(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, provider));
        }
    }

    public interface Flag<TRequest, TResponse, THandler> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        protected static void RegisterInternal(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterInternal(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, THandler>(request, provider));
        }
    }
}

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

    public static class WithFlag<TFeatureFlag>
        where TFeatureFlag : IFeatureFlag
    {
        public static Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
            TRequest request,
            IServiceProvider provider)
            where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {
            return Dispatch(
                request,
                provider.GetRequiredService<THandler>(),
                provider.GetRequiredService<IFeatureManager>(),
                provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
        }

        public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
            TRequest request,
            THandler self,
            IFeatureManager featureManager,
            IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
            where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {
            var isEnabled = await featureManager.IsEnabledAsync(TFeatureFlag.FeatureName);
            if(isEnabled == false)
            {
                return new Disabled();
            }

            var result = await InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, self, pipelines);
            return result.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
        }
    }
}