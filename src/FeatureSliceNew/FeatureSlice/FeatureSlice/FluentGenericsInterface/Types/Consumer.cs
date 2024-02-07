using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics.Interfaces;

public interface IConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Error>>>
{
}

public static class ConsumerFeatureSlice
{
    public interface Default<TRequest, TConsumer> : DelegateFeatureSlice.Default<TRequest, Success>
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterInternal(IServiceCollection services)
        {
            services.AddSingleton<TConsumer>();
            RegisterInternal(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, TConsumer>(request, provider));
        }
    }

    public interface Flag<TRequest, TConsumer> : DelegateFeatureSlice.Flag<TRequest, Success>
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterInternal(IServiceCollection services)
        {
            services.AddSingleton<TConsumer>();
            RegisterInternal(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, TConsumer>(request, provider));
        }
    }
}