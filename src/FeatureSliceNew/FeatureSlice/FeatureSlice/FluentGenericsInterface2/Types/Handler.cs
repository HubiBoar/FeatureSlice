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
    public static class Default<TRequest, TResponse>
    {
        public static void Register<TDispatch, THandler>(IServiceCollection services, DelegateFeatureSlice.Default<TRequest, TResponse>.DispatchConverter<TDispatch> converter)
            where TDispatch : Delegate
            where THandler : class, IHandler<TRequest, TResponse>
        {
            services.AddSingleton<THandler>();
            DelegateFeatureSlice.Default<TRequest, TResponse>.Register(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, provider), converter);
        }
    }

    public static class Flag<TRequest, TResponse>
    {
        public static void Register<TFeatureFlag, TDispatch, THandler>(IServiceCollection services, DelegateFeatureSlice.Flag<TRequest, TResponse>.DispatchConverter<TDispatch> converter)
            where TFeatureFlag : IFeatureFlag
            where TDispatch : Delegate
            where THandler : class, IHandler<TRequest, TResponse>
        {
            services.AddSingleton<THandler>();
            DelegateFeatureSlice.Flag<TRequest, TResponse>.Register<TFeatureFlag, TDispatch>(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, THandler>(request, provider), converter);
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