using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Definit.Endpoint;
using Definit.Dependencies;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
        where TRequest : notnull
        where TResponse : notnull
    {
        public static partial class WithEndpoint
        {
            public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, TResponse, TDependencies>, IEndpointProvider
                where TSelf : Build<TSelf>, new()
            {
                static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

                protected abstract Endpoint Endpoint { get; }

                public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
                {
                    var self = new TSelf();
                    var handle = self.Handle;
                    var serviceLifetime = self.ServiceLifetime;
                    var endpoint = self.Endpoint;

                    services
                        .FeatureSlice()
                        .WithHandler<Dispatch, TRequest, TResponse, TDependencies>
                        (
                            (request, dep) => handle(request, dep),
                            h => h.Invoke,
                            serviceLifetime
                        )
                        .WithEndpoint(extender, endpoint);
                }
            }
        }
    }

    public static partial class WithHandler<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
        where TRequest : notnull
    {
        public static partial class WithEndpoint
        {
            public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, TDependencies>, IEndpointProvider
                where TSelf : Build<TSelf>, new()
            {
                static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

                protected abstract Endpoint Endpoint { get; }

                public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
                {
                    var self = new TSelf();
                    var handle = self.Handle;
                    var serviceLifetime = self.ServiceLifetime;
                    var endpoint = self.Endpoint;

                    services
                        .FeatureSlice()
                        .WithHandler<Dispatch, TRequest, TDependencies>
                        (
                            (request, dep) => handle(request, dep),
                            h => h.Invoke,
                            serviceLifetime
                        )
                        .WithEndpoint(extender, endpoint);
                }
            }
        }
    }
}