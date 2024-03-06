using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;
using Definit.Dependencies;

namespace FeatureSlice;

public sealed record ConsumerName(string Name);

public static class FeatureSliceConsumer
{
    public static class Default
    {
        public static void AddConsumer<TDispatcher, TRequest>(
            IServiceCollection services,
            Messaging.ISetup setup,
            ConsumerName consumerName,
            ServiceFactory<Handler<TRequest, Success>> handlerFactory,
            Func<IServiceProvider, Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
        {
            var dispatch = setup.Register<TRequest>(consumerName, ConvertToConsume);
            services.Add<TDispatcher>(serviceLifetime, ConvertToDispatcher);
            IPublisher.RegisterListener(services, ConvertToListener);
            IPublisher.Register(services);

            Messaging.Consume<TRequest> ConvertToConsume(IServiceProvider provider)
            {
                return HandlerHelper.ConvertToConsume(HandlerHelper.RunWithPipelines(provider, handlerFactory));
            }

            TDispatcher ConvertToDispatcher(IServiceProvider provider)
            {
                return dispatcherConverter(provider, dispatch(provider));
            }

            IPublisher.Listen<TRequest> ConvertToListener(IServiceProvider provider)
            {
                return HandlerHelper.ConvertToListener(dispatch(provider));
            }
        }

        public static void AddConsumer<TDispatcher, TRequest>(
            IServiceCollection services,
            Messaging.ISetup setup,
            ConsumerName consumerName,
            ServiceFactory<Handler<TRequest, Success>> handlerFactory,
            Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
        {
            AddConsumer(services, setup, consumerName, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddConsumer<TDispatcher, TRequest, TDependencies>(
            IServiceCollection services,
            Messaging.ISetup setup,
            ConsumerName consumerName,
            Handler<TRequest, Success, TDependencies> handler,
            Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddConsumer(services, setup, consumerName, provider => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddConsumer<TDispatcher, TRequest>(
            IServiceCollection services,
            Messaging.ISetup setup,
            ConsumerName consumerName,
            Handler<TRequest, Success, FromServicesProvider> handler,
            Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
        {
            AddConsumer(services, setup, consumerName, provider => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }   
    }

    public static class Flag
    {
        public static void AddConsumer<TDispatcher, TRequest>(
            IServiceCollection services,
            Messaging.ISetup setup,
            string featureName,
            ConsumerName consumerName,
            ServiceFactory<Handler<TRequest, Success>> handlerFactory,
            Func<IServiceProvider, Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
        {
            var dispatch = setup.Register<TRequest>(consumerName, ConvertToConsume);
            services.Add<TDispatcher>(serviceLifetime, ConvertToDispatcher);
            IPublisher.RegisterListener(services, ConvertToListener);
            IPublisher.Register(services);

            Messaging.Consume<TRequest> ConvertToConsume(IServiceProvider provider)
            {
                return HandlerHelper.RunWithPipelinesAndFlag(featureName, provider, handlerFactory).Invoke;
            }

            TDispatcher ConvertToDispatcher(IServiceProvider provider)
            {
                return dispatcherConverter(provider, dispatch(provider));
            }

            IPublisher.Listen<TRequest> ConvertToListener(IServiceProvider provider)
            {
                return HandlerHelper.ConvertToListener(dispatch(provider));
            }
        }

        public static void AddConsumer<TDispatcher, TRequest>(
            IServiceCollection services,
            Messaging.ISetup setup,
            string featureName,
            ConsumerName consumerName,
            ServiceFactory<Handler<TRequest, Success>> handlerFactory,
            Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
        {
            AddConsumer(services, setup, featureName, consumerName, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddConsumer<TDispatcher, TRequest, TDependencies>(
            IServiceCollection services,
            Messaging.ISetup setup,
            string featureName,
            ConsumerName consumerName,
            Handler<TRequest, Success, TDependencies> handler,
            Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddConsumer(services, setup, featureName, consumerName, provider => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddConsumer<TDispatcher, TRequest>(
            IServiceCollection services,
            Messaging.ISetup setup,
            string featureName,
            ConsumerName consumerName,
            Handler<TRequest, Success, FromServicesProvider> handler,
            Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TRequest : notnull
        {
            AddConsumer(services, setup, featureName, consumerName, provider => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }
    }
}