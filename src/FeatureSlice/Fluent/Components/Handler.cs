using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public static class FeatureSliceHandler
{
    public static class Default
    {
        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
            Func<IServiceProvider, Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
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
            ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
            Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse, TDependencies>(
            IServiceCollection services,
            Handler<TRequest, Result<TResponse>, TDependencies> handler,
            Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
            where TDispatcher : Delegate
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddHandler(services, (provider) => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            Handler<TRequest, Result<TResponse>, FromServicesProvider> handler,
            Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, (provider) => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }



        public static void AddHandler<TDispatcher, TRequest>(
            IServiceCollection services,
            ServiceFactory<Handler<TRequest, Result>> handlerFactory,
            Func<IServiceProvider, Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
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

        public static void AddHandler<TDispatcher, TRequest>(
            IServiceCollection services,
            ServiceFactory<Handler<TRequest, Result>> handlerFactory,
            Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TDependencies>(
            IServiceCollection services,
            Handler<TRequest, Result, TDependencies> handler,
            Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TDispatcher : Delegate
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddHandler(services, (provider) => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest>(
            IServiceCollection services,
            Handler<TRequest, Result, FromServicesProvider> handler,
            Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
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
            ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
            Func<IServiceProvider, Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
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
            ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
            Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, featureName, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse, TDependencies>(
            IServiceCollection services,
            string featureName,
            Handler<TRequest, Result<TResponse>, TDependencies> handler,
            Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
            where TDispatcher : Delegate
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddHandler(services, featureName, (provider) => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TResponse>(
            IServiceCollection services,
            string featureName,
            Handler<TRequest, Result<TResponse>, FromServicesProvider> handler,
            Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TResponse : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, featureName, (provider) => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }



        public static void AddHandler<TDispatcher, TRequest>(
            IServiceCollection services,
            string featureName,
            ServiceFactory<Handler<TRequest, Result>> handlerFactory,
            Func<IServiceProvider, Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
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

        public static void AddHandler<TDispatcher, TRequest>(
            IServiceCollection services,
            string featureName,
            ServiceFactory<Handler<TRequest, Result>> handlerFactory,
            Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, featureName, handlerFactory, (_, handler) => dispatcherConverter(handler), serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest, TDependencies>(
            IServiceCollection services,
            string featureName,
            Handler<TRequest, Result, TDependencies> handler,
            Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TDispatcher : Delegate
            where TDependencies : class, IFromServices<TDependencies>
        {
            AddHandler(services, featureName, (provider) => request => handler(request, TDependencies.Create(provider)), dispatcherConverter, serviceLifetime);
        }

        public static void AddHandler<TDispatcher, TRequest>(
            IServiceCollection services,
            string featureName,
            Handler<TRequest, Result, FromServicesProvider> handler,
            Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TRequest : notnull
            where TDispatcher : Delegate
        {
            AddHandler(services, featureName, (provider) => request => handler(request, provider.From()), dispatcherConverter, serviceLifetime);
        }
    }
}