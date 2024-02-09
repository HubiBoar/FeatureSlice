using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class FeatureSlice
{
    public static partial class AsEndpoint
    {
        public static class AsHandler<TRequest, TResponse, TDependencies>
            where TDependencies : class, IFromServices<TDependencies>
        {
            public abstract class BuildAs<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf, TDependencies>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse, TDependencies>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse, TDependencies>
                where TSelf : Build<TSelf>, new()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }
                protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies) => new TSelf().Handle(request, dependencies);
            }    
        }
    }
}