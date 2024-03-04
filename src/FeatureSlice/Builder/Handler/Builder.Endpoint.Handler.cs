using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithEndpoint
    {
        public static partial class WithHandler<TRequest, TResponse, TDependencies>
            where TDependencies : class, IFromServices<TDependencies>
            where TRequest : notnull
            where TResponse : notnull
        {
            public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, TResponse, TDependencies>, IEndpoint
                where TSelf : Build<TSelf>, new()
            {
                static IEndpoint.Setup IEndpoint.Endpoint { get; } = new TSelf().Endpoint;

                protected abstract IEndpoint.Setup Endpoint { get; }

                public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
                {
                    var self = new TSelf();
                    var handle = self.Handle;
                    var serviceLifetime = self.ServiceLifetime;
                    var endpoint = self.Endpoint;

                    services
                        .FeatureSlice()
                        .WithEndpoint(extender, endpoint)
                        .WithHandler<Dispatch, TRequest, TResponse, TDependencies>(
                            (request, dep) => handle(request, dep),
                            h => h.Invoke,
                            serviceLifetime);
                }
            }
        }
    }
}