using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static class StaticHandlerFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse, THandler, TDependencies> : DelegateFeatureSlice.Default<TRequest, TResponse>
        where THandler : class, IStaticHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            RegisterBase(services, provider => request => InMemoryDispatcher<TRequest, TResponse>.Default.Dispatch(request, provider, r => THandler.Dispatch<THandler>(r, provider)));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TResponse, THandler, TDependencies> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where TFeatureFlag : IFeatureFlag
        where THandler : class, IStaticHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            RegisterBase(services, provider => request => InMemoryDispatcher<TRequest, TResponse>.WithFlag.Dispatch(request, provider, r => THandler.Dispatch<THandler>(r, provider), TFeatureFlag.FeatureName));
        }
    }
}