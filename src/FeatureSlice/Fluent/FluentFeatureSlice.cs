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

        public IWithEndpoint WithEndpoint(IHostExtender<WebApplication> extender, Func<IEndpointRouteBuilder, IEndpointConventionBuilder> endpoint)
        {
            FeatureSliceEndpoint.AddEndpoint(extender, endpoint);
            return new FluentFeatureSliceWithEndpoint(this);
        }

        public IWithEndpoint WithEndpoint(IHostExtender<WebApplication> extender, Endpoint endpoint)
        {
            FeatureSliceEndpoint.AddEndpoint(extender, endpoint);
            return new FluentFeatureSliceWithEndpoint(this);
        }
    }

    public interface IWithEndpoint : AddConsumer.IDefault, AddHandler.IDefault
    {
    }

    public interface IWithFlag : AddConsumer.IWithFlag, AddHandler.IWithFlag
    {
        public interface IWithEndpoint : AddConsumer.IWithFlag, AddHandler.IWithFlag 
        {
        }

        public IWithEndpoint WithEndpoint(IHostExtender<WebApplication> extender, Func<IEndpointRouteBuilder, IEndpointConventionBuilder> endpoint)
        {
            FeatureSliceEndpoint.AddEndpoint(extender, endpoint);
            return new FluentFeatureSliceWithEndpointAndFlag(this);
        }

        public IWithEndpoint WithEndpoint(IHostExtender<WebApplication> extender, Endpoint endpoint)
        {
            FeatureSliceEndpoint.AddEndpoint(extender, endpoint);
            return new FluentFeatureSliceWithEndpointAndFlag(this);
        }
    }

    public static class AddConsumer
    {
        public interface IDefault : IFluentFeatureSlice
        {
            public void WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Default.AddConsumer(Services, setup, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Default.AddConsumer(Services, setup, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithConsumer<TDispatcher, TRequest, TDependencies>(
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
            }
        }

        public interface IWithFlag : IFluentFeatureSlice.IFeatureName
        {
            public void WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Flag.AddConsumer(Services, setup, FeatureName, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithConsumer<TDispatcher, TRequest>(
                Messaging.ISetup setup,
                ConsumerName consumerName,
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Messaging.Dispatch<TRequest>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TDispatcher : Delegate
                where TRequest : notnull
            {
                FeatureSliceConsumer.Flag.AddConsumer(Services, setup, FeatureName, consumerName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithConsumer<TDispatcher, TRequest, TDependencies>(
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
            }
        }
    }

    public static class AddHandler
    {
        public interface IDefault : IFluentFeatureSlice
        {
            public void WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TResponse, TDependencies>(
                Handler<TRequest, Result<TResponse>, TDependencies> handler,
                Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TResponse>(
                Handler<TRequest, Result<TResponse>, FromServicesProvider> handler,
                Func<Handler<TRequest, Result<TResponse>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
            }




            public void WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TDependencies>(
                Handler<TRequest, Result, TDependencies> handler,
                Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest>(
                Handler<TRequest, Result, FromServicesProvider> handler,
                Func<Handler<TRequest, Result>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Default.AddHandler(Services, handler, dispatcherConverter, serviceLifetime);
            }
        }

        public interface IWithFlag : IFluentFeatureSlice.IFeatureName
        {
            public void WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TResponse>(
                ServiceFactory<Handler<TRequest, Result<TResponse>>> handlerFactory,
                Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TResponse, TDependencies>(
                Handler<TRequest, Result<TResponse>, TDependencies> handler,
                Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TResponse>(
                Handler<TRequest, Result<TResponse>, FromServicesProvider> handler,
                Func<Handler<TRequest, Result<TResponse, Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TResponse : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
            }



            public void WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<IServiceProvider, Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest>(
                ServiceFactory<Handler<TRequest, Result>> handlerFactory,
                Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handlerFactory, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest, TDependencies>(
                Handler<TRequest, Result, TDependencies> handler,
                Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
                where TDependencies : class, IFromServices<TDependencies>
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
            }

            public void WithHandler<TDispatcher, TRequest>(
                Handler<TRequest, Result, FromServicesProvider> handler,
                Func<Handler<TRequest, Result.Or<Disabled>>, TDispatcher> dispatcherConverter,
                ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
                where TRequest : notnull
                where TDispatcher : Delegate
            {
                FeatureSliceHandler.Flag.AddHandler(Services, FeatureName, handler, dispatcherConverter, serviceLifetime);
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

internal sealed class FluentFeatureSliceWithEndpoint : FluentFeatureSlice.IWithEndpoint
{
    public IServiceCollection Services { get; }

    public FluentFeatureSliceWithEndpoint(FluentFeatureSlice.IInitial builder)
    {
        Services = builder.Services;
    }
}

internal sealed class FluentFeatureSliceWithEndpointAndFlag : FluentFeatureSlice.IWithFlag.IWithEndpoint
{
    public IServiceCollection Services { get; }
    public string FeatureName { get; }

    public FluentFeatureSliceWithEndpointAndFlag(FluentFeatureSlice.IWithFlag feature)
    {
        Services = feature.Services;
        FeatureName = feature.FeatureName;
    }
}
