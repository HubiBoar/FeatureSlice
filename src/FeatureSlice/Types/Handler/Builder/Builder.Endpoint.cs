using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsEndpoint
    {
        public static class WithHandler<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public abstract class BuildAs<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, WebAppExtender hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint
                where TSelf : Build<TSelf>, new ()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
            }
        }
    }
}