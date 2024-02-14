using Explicit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IStaticHandler<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
{
    public virtual static Task<OneOf<TResponse, Error>> Dispatch<THandler>(TRequest request, IServiceProvider provider)
        where THandler : IStaticHandler<TRequest, TResponse, TDependencies>
    {
        return THandler.Handle(request, TDependencies.Create(provider));
    }

    public static abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
}

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

    public abstract partial class Flag<TFeatureName, TRequest, TResponse, THandler, TDependencies> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where TFeatureName : IFeatureName
        where THandler : class, IStaticHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            RegisterBase(services, provider => request => InMemoryDispatcher<TRequest, TResponse>.WithFlag.Dispatch(request, provider, r => THandler.Dispatch<THandler>(r, provider), TFeatureName.FeatureName));
        }
    }
}