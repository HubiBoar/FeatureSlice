using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class FeatureSliceHandler
{
    public static class Default
    {
        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            ServiceFactory<Handler<TRequest, TResponse>> handlerFactory,
            Func<IServiceProvider, Handler<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
        {
            services.Add<TDispatcher>(serviceLifetime, ConvertToDispatcher);
            IPublisher.RegisterListener(services, ConvertToListener);
            IPublisher.Register(services);

            TDispatcher ConvertToDispatcher(IServiceProvider provider)
            {
                return dispatcherConverter(provider, HandlerHelper.RunWithPipelines(provider, handlerFactory));
            }

            IPublisher.Listen<TRequest> ConvertToListener(IServiceProvider provider)
            {
                return HandlerHelper.ConvertToListener(handlerFactory(provider));
            }
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            ServiceFactory<Handler<TRequest, TResponse>> handlerFactory,
            Func<Handler<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
        {
            AddHandler(services, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse, TDependencies>(
            IServiceCollection services,
            Handler<TRequest, TResponse, TDependencies> handler,
            Func<Handler<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddHandler(services, (provider) => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            Handler<TRequest, TResponse, FromServiceProvider> handler,
            Func<Handler<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
        {
            AddHandler(services, (provider) => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }
    }

    public static class Flag
    {
        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            string featureName,
            ServiceFactory<Handler<TRequest, TResponse>> handlerFactory,
            Func<IServiceProvider, HandlerWithFlag<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
        {
            services.Add<TDispatcher>(serviceLifetime, ConvertToDispatcher);
            IPublisher.RegisterListener(services, ConvertToListener);
            IPublisher.Register(services);

            TDispatcher ConvertToDispatcher(IServiceProvider provider)
            {
                return dispatcherConverter(provider, HandlerHelper.RunWithPipelinesAndFlag(featureName, provider, handlerFactory));
            }

            IPublisher.Listen<TRequest> ConvertToListener(IServiceProvider provider)
            {
                return HandlerHelper.ConvertToListener(handlerFactory(provider));
            }
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            string featureName,
            ServiceFactory<Handler<TRequest, TResponse>> handlerFactory,
            Func<HandlerWithFlag<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
        {
            AddHandler(services, featureName, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse, TDependencies>(
            IServiceCollection services,
            string featureName,
            Handler<TRequest, TResponse, TDependencies> handler,
            Func<HandlerWithFlag<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddHandler(services, featureName, (provider) => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            string featureName,
            Handler<TRequest, TResponse, FromServiceProvider> handler,
            Func<HandlerWithFlag<TRequest, TResponse>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDispatcher : Delegate
        {
            AddHandler(services, featureName, (provider) => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }
    }
}