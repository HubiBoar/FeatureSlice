using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithFlag
    {
        public static partial class WithEndpoint
        {
            public static partial class WithHandler<TRequest, TResponse, TDependencies>
                where TDependencies : class, IFromServices<TDependencies>
                where TRequest : notnull
                where TResponse : notnull
            {
                public abstract class Build<TSelf> : HandlerBaseWithFlag<TSelf, TRequest, TResponse, TDependencies>, IEndpoint
                    where TSelf : Build<TSelf>, new()
                {
                    static IEndpoint.Setup IEndpoint.Endpoint { get; } = new TSelf().Endpoint;

                    protected abstract IEndpoint.Setup Endpoint { get; }

                    public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
                    {
                        var self = new TSelf();
                        var featureName = self.FeatureName;
                        var handle = self.Handle;
                        var serviceLifetime = self.ServiceLifetime;
                        var endpoint = self.Endpoint;

                        services
                            .FeatureSlice()
                            .WithFlag(featureName)
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
}