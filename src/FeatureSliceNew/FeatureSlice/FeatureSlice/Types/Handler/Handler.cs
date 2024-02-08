using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
{
}

public static class HandlerFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse, THandler> : DelegateFeatureSlice.Default<TRequest, TResponse>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, provider));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TResponse, THandler> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where TFeatureFlag : IFeatureFlag
        where THandler : class, IHandler<TRequest, TResponse>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher.WithFlag.Dispatch<TRequest, TResponse, THandler>(request, provider, TFeatureFlag.FeatureName));
        }
    }
}