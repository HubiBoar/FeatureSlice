using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Endpoint;
using Definit.Results;

namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public static partial class WithConsumer
    {
        public static partial class WithEndpoint
        {
            public abstract class Build<TSelf> : ConsumerBase<TSelf, TRequest, Result<TResponse>, TResponse, TDependencies>, IEndpointProvider
                where TSelf : Build<TSelf>, IEndpointProvider, new()
            {
                static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

                protected abstract Endpoint Endpoint { get; }

                public static void Register
                (
                    IServiceCollection services,
                    IConsumerSetup setup,
                    IHostExtender<WebApplication> extender
                )
                {
                    RegisterConsumer(services, setup);
                    extender.Map(TSelf.Endpoint);
                }

                public static void Register
                (
                    IServiceCollection services,
                    IHostExtender<WebApplication> extender
                )
                {
                    RegisterConsumer(services);
                    extender.Map(TSelf.Endpoint);
                }

                public static void Register
                (
                    IServiceCollection services,
                    IHostExtender<WebApplication> extender,
                    ServiceFactory<IHandlerSetup> handlingSetupFactory,
                    ServiceFactory<IConsumerSetup> consumerSetupFactory
                )
                {
                    RegisterConsumer(services, handlingSetupFactory, consumerSetupFactory);
                    extender.Map(TSelf.Endpoint);
                }
            }
        }
    }
}

public static partial class FeatureSlice<TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    public static partial class WithConsumer
    {
        public static partial class WithEndpoint
        {
            public abstract class Build<TSelf> : ConsumerBase<TSelf, TRequest, Result, Success, TDependencies>, IEndpointProvider
                where TSelf : Build<TSelf>, IEndpointProvider, new()
            {
                static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

                protected abstract Endpoint Endpoint { get; }

                public static void Register
                (
                    IServiceCollection services,
                    IConsumerSetup setup,
                    IHostExtender<WebApplication> extender
                )
                {
                    RegisterConsumer(services, setup);
                    extender.Map(TSelf.Endpoint);
                }

                public static void Register
                (
                    IServiceCollection services,
                    IHostExtender<WebApplication> extender
                )
                {
                    RegisterConsumer(services);
                    extender.Map(TSelf.Endpoint);
                }

                public static void Register
                (
                    IServiceCollection services,
                    IHostExtender<WebApplication> extender,
                    ServiceFactory<IHandlerSetup> handlingSetupFactory,
                    ServiceFactory<IConsumerSetup> consumerSetupFactory
                )
                {
                    RegisterConsumer(services, handlingSetupFactory, consumerSetupFactory);
                    extender.Map(TSelf.Endpoint);
                }
            }
        }
    }
}