using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsEndpoint
    {
        public static class WithConsumer<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public abstract class BuildAs<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services, setupProvider.GetSetup);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint
                where TSelf : Build<TSelf>, new()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
            }
        }
    }
}