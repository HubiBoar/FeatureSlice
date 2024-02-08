using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IConsumer<TRequest>
{
    public Task<OneOf<Success, Error>> Consume(TRequest request);
}

public static class ConsumerFeatureSlice
{
    public abstract partial class Default<TRequest, TConsumer> : DelegateFeatureSlice.Default<TRequest, Success>
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, Success, TConsumer>(request, provider));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TConsumer> : DelegateFeatureSlice.Flag<TRequest, Success>
        where TFeatureFlag : IFeatureFlag
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterBase(IServiceCollection services)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(services, provider => request => InMemoryDispatcher.WithFlag.Dispatch<TRequest, Success, TConsumer>(request, provider, TFeatureFlag.FeatureName));
        }
    }
}