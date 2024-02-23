using Definit.Configuration;
using Microsoft.AspNetCore.Builder;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsFlag
    {
        public static partial class AsEndpoint
        {
            public abstract class BuildAs<TSelf> : EndpointFeatureSlice.Flag<TSelf, TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureName
            {
                public static void Register(WebAppExtender hostExtender)
                {
                    EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureName
                where TSelf : Build<TSelf>, new()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }
                protected abstract string FeatureName { get; }

                static string IFeatureName.FeatureName => new TSelf().FeatureName;
                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
            }
        }
    }
}