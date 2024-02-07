using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.FeatureManagement;

namespace FeatureSlice.FluentGenerics.Interfaces2;

public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
{
}

public static class HandlerFeatureSlice
{
    public interface Default<TRequest, TResponse> : DelegateFeatureSlice.Default<TRequest, TResponse>
    {
        protected static void RegisterBase<TDispatch, THandler>(IServiceCollection services, DispatchConverter<TDispatch> converter)
            where TDispatch : Delegate
            where THandler : class, IHandler<TRequest, TResponse>
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, provider), converter);
        }
    }

    public interface Flag<TRequest, TResponse> : DelegateFeatureSlice.Flag<TRequest, TResponse>
    {
        protected static void RegisterBase<TFeatureFlag, TDispatch, THandler>(IServiceCollection services, DispatchConverter<TDispatch> converter)
            where TFeatureFlag : IFeatureFlag
            where TDispatch : Delegate
            where THandler : class, IHandler<TRequest, TResponse>
        {
            services.AddSingleton<THandler>();
            RegisterBase<TFeatureFlag, TDispatch>(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, THandler>(request, provider), converter);
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