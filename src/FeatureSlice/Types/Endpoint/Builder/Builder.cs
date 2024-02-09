using Microsoft.AspNetCore.Builder;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsEndpoint
    {
        public abstract class BuildAs<TSelf> : EndpointFeatureSlice.Default<TSelf>
            where TSelf : BuildAs<TSelf>, IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
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