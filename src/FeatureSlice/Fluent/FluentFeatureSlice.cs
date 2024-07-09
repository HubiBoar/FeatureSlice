using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Endpoint;
using Definit.Results;

namespace FeatureSlice;

public interface IFluentFeatureSlice
{
    public IServiceCollection Services { get; }

    public interface IFeatureName : IFluentFeatureSlice
    {
        public string FeatureName { get; }
    }
}

public static class FluentFeatureSlice
{
    public static IInitial Create(IServiceCollection services)
    {
        return new FluentFeatureSliceInitial(services);
    }

    public interface IInitial : AddConsumer.IDefault, AddHandler.IDefault
    {
        public IWithFlag WithFlag(string featureName)
        {
            return new FluentFeatureSliceWithFlag(this, featureName);
        }
    }

    public interface ICanHaveEndpoint : IFluentFeatureSlice
    {
        public void WithEndpoint(IHostExtender<WebApplication> extender, Func<IEndpointRouteBuilder, IEndpointConventionBuilder> endpoint)
        {
            FeatureSliceEndpoint.AddEndpoint(extender, endpoint);
        }

        public void WithEndpoint(IHostExtender<WebApplication> extender, Endpoint endpoint)
        {
            FeatureSliceEndpoint.AddEndpoint(extender, endpoint);
        }
    }

    public interface IWithFlag : AddConsumer.IWithFlag, AddHandler.IWithFlag
    {
    }

    public static class AddConsumer
    {
        public interface IDefault : IFluentFeatureSlice
        {
            public ICanHaveEndpoint WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Default.AddConsumer(Services, setup, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Default.AddConsumer(Services, setup, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithConsumer<TDispatcher, TRequest, TDependencies>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                Handler<TRequest, Result, TDependencies> handler,
                Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceConsumer.Default.AddConsumer(Services, setup, consumerName, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }
        }

        public interface IWithFlag : IFluentFeatureSlice.IFeatureName
        {
            public ICanHaveEndpoint WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Flag.AddConsumer(Services, setup, FeatureName, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Flag.AddConsumer(Services, setup, FeatureName, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithConsumer<TDispatcher, TRequest, TDependencies>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                Handler<TRequest, Result, TDependencies> handler,
                Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceConsumer.Flag.AddConsumer(Services, setup, FeatureName, consumerName, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }
        }
    }

    public static class AddHandler
    {
        public interface IDefault : IFluentFeatureSlice
        {
            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse, TDependencies>(
                Handler<TRequest, Result<TResponse>, TDependencies> handler,
                Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse>(
                Handler<TRequest, Result<TResponse>, FromServicesProvider> handler,
                Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TDependencies>(
                Handler<TRequest, Result, TDependencies> handler,
                Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest>(
                Handler<TRequest, Result, FromServicesProvider> handler,
                Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }
        }

        public interface IWithFlag : IFluentFeatureSlice.IFeatureName
        {
            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse, TDependencies>(
                Handler<TRequest, Result<TResponse>, TDependencies> handler,
                Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TResponse>(
                Handler<TRequest, Result<TResponse>, FromServicesProvider> handler,
                Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest, TDependencies>(
                Handler<TRequest, Result, TDependencies> handler,
                Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }

            public ICanHaveEndpoint WithHandler<TDispatcher, TRequest>(
                Handler<TRequest, Result, FromServicesProvider> handler,
                Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
                return new FluentFeatureSliceEndpoint(Services);
            }
        }
    }
}


internal sealed class FluentFeatureSliceInitial : FluentFeatureSlice.IInitial
{
    public IServiceCollection Services { get; }

    public FluentFeatureSliceInitial(IServiceCollection services)
    {
        Services = services;
    }
}

internal sealed class FluentFeatureSliceWithFlag : FluentFeatureSlice.IWithFlag
{
    public IServiceCollection Services { get; }

    public string FeatureName { get; }

    public FluentFeatureSliceWithFlag(FluentFeatureSlice.IInitial builder, string featureName)
    {
        Services = builder.Services;
        FeatureName = featureName;
    }
}

internal sealed class FluentFeatureSliceEndpoint : FluentFeatureSlice.ICanHaveEndpoint
{
    public IServiceCollection Services { get; }

    public FluentFeatureSliceEndpoint(IServiceCollection services)
    {
        Services = services;
    }
}
