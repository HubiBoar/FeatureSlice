using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Definit.Endpoint;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public static partial class WithEndpoint
    {
        public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, Result<TResponse>, TResponse, TDependencies>, IEndpointProvider
            where TSelf : Build<TSelf>, IEndpointProvider, new()
        {
            static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

            protected abstract Endpoint Endpoint { get; }

            public static void Register
            (
                IServiceCollection services,
                IHostExtender<WebApplication> extender
            )
            {
                RegisterHandler(services);
                extender.Map(TSelf.Endpoint);
            }
    
            public static void Register
            (
                IServiceCollection services,
                IHostExtender<WebApplication> extender,
                ServiceFactory<IHandlerSetup> setupFactory 
            )
            {
                RegisterHandler(services, setupFactory);
                extender.Map(TSelf.Endpoint);
            }
        }
    }
}

public static partial class FeatureSlice<TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    public static partial class WithEndpoint
    {
        public abstract class Build<TSelf> : 
            FeatureSlice<TRequest, Result, TDependencies>
            .WithEndpoint
            .Build<TSelf>
            where TSelf : Build<TSelf>, new()
        {
        }
    }
}