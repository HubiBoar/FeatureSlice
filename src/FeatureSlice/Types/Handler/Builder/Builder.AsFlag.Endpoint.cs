using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsFlag
    {
        public static partial class AsEndpoint
        {
            public static class WithHandler<TRequest, TResponse, THandler>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public abstract class BuildAs<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                    where TSelf : Build<TSelf>, new ()
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