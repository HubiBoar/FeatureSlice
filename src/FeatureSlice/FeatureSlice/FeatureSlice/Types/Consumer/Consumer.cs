using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public sealed record ConsumerName(string Name);

public interface IConsumer<TRequest>
{
    public abstract static ConsumerName ConsumerName { get; }

    public Task<OneOf<Success, Error>> Consume(TRequest request);
}

public static class ConsumerFeatureSlice
{
    public abstract partial class Default<TRequest, TConsumer> : DelegateFeatureSlice.Default<TRequest, Success>
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterBase(IServiceCollection services, Func<IServiceProvider, Messaging.ISetup> getSetup)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(
                services,
                Messaging.Dispatcher<TRequest>.Default.Register(
                    services,
                    TConsumer.ConsumerName,
                    provider => provider.GetRequiredService<TConsumer>().Consume,
                    getSetup));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TConsumer> : DelegateFeatureSlice.Flag<TRequest, Success>
        where TFeatureFlag : IFeatureFlag
        where TConsumer : class, IConsumer<TRequest>
    {
        protected static void RegisterBase(IServiceCollection services, Func<IServiceProvider, Messaging.ISetup> getSetup)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(
                services,
                Messaging.Dispatcher<TRequest>.WithFlag.Register(
                    services,
                    TConsumer.ConsumerName,
                    TFeatureFlag.FeatureName,
                    provider => provider.GetRequiredService<TConsumer>().Consume,
                    getSetup));
        }
    }
}