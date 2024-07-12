using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;


namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    
    public static partial class WithConsumer
    {
        public abstract class Build<TSelf> : ConsumerBase<TSelf, TRequest, Result<TResponse>, TResponse, TDependencies>
            where TSelf : Build<TSelf>, new()
        {
            public static void Register(IServiceCollection services, IConsumerSetup setup)
            {
                RegisterConsumer(services, setup);
            }

            public static void Register(IServiceCollection services)
            {
                RegisterConsumer(services);
            }

            public static void Register
            (
                IServiceCollection services,
                ServiceFactory<IHandlerSetup> handlingSetupFactory,
                ServiceFactory<IConsumerSetup> consumerSetupFactory
            )
            {
                RegisterConsumer(services, handlingSetupFactory, consumerSetupFactory);
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
        public abstract class Build<TSelf> : 
            FeatureSlice<TRequest, Result, TDependencies>
            .WithConsumer
            .Build<TSelf>
            where TSelf : Build<TSelf>, new()
        {
        }
    }
}