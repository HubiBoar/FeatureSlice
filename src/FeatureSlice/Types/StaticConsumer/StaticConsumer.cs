using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IStaticConsumer<TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
{
    public abstract static ConsumerName ConsumerName { get; }

    public virtual static Task<OneOf<Success, Error>> Dispatch<TConsumer>(TRequest request, IServiceProvider provider)
        where TConsumer : IStaticConsumer<TRequest, TDependencies>
    {
        return TConsumer.Consume(request, TDependencies.Create(provider));
    }

    public static abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);
}

public static class StaticConsumerFeatureSlice
{
    public abstract partial class Default<TRequest, TConsumer, TDependencies> : DelegateFeatureSlice.Default<TRequest, Success>
        where TConsumer : class, IStaticConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services, Func<IServiceProvider, Messaging.ISetup> getSetup)
        {
            RegisterBase(
                services,
                Messaging.Dispatcher<TRequest>.Default.Register(
                    services,
                    TConsumer.ConsumerName,
                    provider => message => TConsumer.Dispatch<TConsumer>(message, provider),
                    getSetup));
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TConsumer, TDependencies> : DelegateFeatureSlice.Flag<TRequest, Success>
        where TFeatureFlag : IFeatureFlag
        where TConsumer : class, IStaticConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
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
                    provider => message => TConsumer.Dispatch<TConsumer>(message, provider),
                    getSetup));
        }
    }
}