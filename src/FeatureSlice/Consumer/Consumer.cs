using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public abstract class ConsumerBase<TSelf, TRequest, TResponse, TResult, TDependencies> :
        HandlerBase<TSelf, TRequest, TResponse, TResult, TDependencies>
    where TSelf : ConsumerBase<TSelf, TRequest, TResponse, TResult, TDependencies>, new()
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : Result_Base<TResult>
    where TResult : notnull
{
    public delegate Task<Result> Consume(TRequest request);

    protected abstract ConsumerName ConsumerName { get; }

    internal static void RegisterConsumer
    (
        IServiceCollection services,
        ServiceFactory<IHandlerSetup> handlingSetupFactory,
        ServiceFactory<IConsumerSetup> consumerSetupFactory
    )
    {
        RegisterHandler(services, handlingSetupFactory);

        var self = new TSelf();
        var serviceLifetime = self.ServiceLifetime;
        var consumerName = self.ConsumerName;
        services.Add(serviceLifetime, GetConsumer);

        Consume GetConsumer(IServiceProvider provider)
        {
            var setup = consumerSetupFactory(provider);
            var handling = handlingSetupFactory(provider);
            var handler = provider.GetRequiredService<Handle>();

            var consumer = setup.GetConsumer<TRequest>
            (
                consumerName,
                provider,
                handling,
                async request => 
                {
                    if((await handler(request))
                        .Is(out Error error))
                    {
                        return error;
                    }

                    return Result.Success;
                }
            );

            return request => consumer(request);
        }
    }

    internal static void RegisterConsumer
    (
        IServiceCollection services
    )
    {
        var consumerFactory = IConsumerSetup.TryRegisterDefault(services);
        var handlerFactory = IHandlerSetup.TryRegisterDefault(services);
        RegisterConsumer(services, handlerFactory, consumerFactory);
    }

    internal static void RegisterConsumer
    (
        IServiceCollection services,
        IConsumerSetup consumerSetup
    )
    {
        var handlerFactory = IHandlerSetup.TryRegisterDefault(services);
        RegisterConsumer(services, handlerFactory, provider => consumerSetup);
    }
}