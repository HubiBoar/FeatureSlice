using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static class StaticHandlerFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse, TDependencies, THandler> : DelegateFeatureSlice.Default<TRequest, TResponse>
        where THandler : class, IStaticHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, provider));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TResponse, TDependencies, THandler> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where TFeatureFlag : IFeatureFlag
        where THandler : class, IStaticHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.WithFlag.Dispatch<TRequest, TResponse, THandler>(request, provider, TFeatureFlag.FeatureName));
        }
    }
}