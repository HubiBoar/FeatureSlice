using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics;

public interface IConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Error>>>
{
}

public static class ConsumerFeatureSlice
{
    public abstract partial class Default<TRequest, TConsumer> : DelegateFeatureSlice.Default<TRequest, Success>
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, TConsumer>(request, provider));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TConsumer> : DelegateFeatureSlice.Flag<TFeatureFlag, TRequest, Success>
        where TFeatureFlag : IFeatureFlag
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, TConsumer>(request, provider));
        }
    }
}