using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static class StaticHandlerFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse, THandler> : DelegateFeatureSlice.Default<TRequest, TResponse>
        where THandler : class, IStaticHandler<TRequest, TResponse>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.Dispatch(request, provider, r => THandler.DispatchWithProvider(r, provider)));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TResponse, THandler> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where TFeatureFlag : IFeatureFlag
        where THandler : class, IStaticHandler<TRequest, TResponse>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.WithFlag.Dispatch(request, provider, r => THandler.DispatchWithProvider(r, provider), TFeatureFlag.FeatureName));
        }
    }
}