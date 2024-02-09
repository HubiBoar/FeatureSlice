using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsFlag
    {
        public static partial class AsEndpoint
        {
            public static class WithConsumer<TRequest, TConsumer>
                where TConsumer : class, IConsumer<TRequest>
            {
                public abstract class BuildAs<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services, setupProvider.GetSetup);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                    where TSelf : Build<TSelf>, new()
                {
                    protected abstract IEndpoint.Setup Endpoint { get; }
                    protected abstract string FeatureName { get; }

                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
                }
            }
        }
    }
}