using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Endpoint;

namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    public static partial class WithFlag
    {
        public static partial class WithEndpoint
        {
            public static partial class WithConsumer
            {
                public abstract class Build<TSelf> : ConsumerBaseWithFlag<TSelf, TRequest, TDependencies>, IEndpointProvider
                    where TSelf : Build<TSelf>, new()
                {
                    static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

                    protected abstract Endpoint Endpoint { get; }

                    public static void Register(IServiceCollection services, Messaging.ISetup setup, IHostExtender<WebApplication> extender)
                    {
                        var self = new TSelf();
                        var featureName = self.FeatureName;
                        var consumerName = self.ConsumerName;
                        var handle = self.Consume;
                        var serviceLifetime = self.ServiceLifetime;
                        var endpoint = self.Endpoint;

                        services
                            .FeatureSlice()
                            .WithFlag(featureName)
                            .WithConsumer<Dispatch, TRequest, TDependencies>
                            (
                                setup,
                                consumerName,
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
}