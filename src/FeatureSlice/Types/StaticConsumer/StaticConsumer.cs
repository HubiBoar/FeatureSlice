using Definit.Configuration;
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
        where TRequest : notnull
        where TConsumer : class, IStaticConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services, Messaging.ISetup setup)
        {
            RegisterBase(
                services,
                Messaging.Dispatcher<TRequest>.Default.Register(
                    TConsumer.ConsumerName,
                    provider => message => TConsumer.Dispatch<TConsumer>(message, provider),
                    setup));
        }
    }

    public abstract partial class Flag<TFeatureName, TRequest, TConsumer, TDependencies> : DelegateFeatureSlice.Flag<TRequest, Success>
        where TFeatureName : IFeatureName
        where TRequest : notnull
        where TConsumer : class, IStaticConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        protected static void RegisterBase(IServiceCollection services, Messaging.ISetup setup)
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(
                services,
                Messaging.Dispatcher<TRequest>.WithFlag.Register(
                    TConsumer.ConsumerName,
                    TFeatureName.FeatureName,
                    provider => message => TConsumer.Dispatch<TConsumer>(message, provider),
                    setup));
        }
    }
}