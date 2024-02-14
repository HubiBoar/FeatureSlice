using Explicit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IHandler<TRequest, TResponse>
{
    public Task<OneOf<TResponse, Error>> Handle(TRequest request);
}

public static class HandlerFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse, THandler> : DelegateFeatureSlice.Default<TRequest, TResponse>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher<TRequest, TResponse>.Default.Dispatch(request, provider, provider.GetRequiredService<THandler>().Handle));
        }
    }

    public abstract partial class Flag<TFeatureName, TRequest, TResponse, THandler> : DelegateFeatureSlice.Flag<TRequest, TResponse>
        where TFeatureName : IFeatureName
        where THandler : class, IHandler<TRequest, TResponse>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<THandler>();
            RegisterBase(services, provider => request => InMemoryDispatcher<TRequest, TResponse>.WithFlag.Dispatch(request, provider, provider.GetRequiredService<THandler>().Handle, TFeatureName.FeatureName));
        }
    }
}