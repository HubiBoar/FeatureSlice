using Explicit.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsFlag
    {
        public static partial class AsEndpoint
        {
            public static class AsConsumer<TRequest, TDependencies>
                where TDependencies : class, IFromServices<TDependencies>
            {
                public abstract class BuildAs<TSelf> : StaticConsumerFeatureSlice.Flag<TSelf, TRequest, TSelf, TDependencies>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureName, IStaticConsumer<TRequest, TDependencies>
                {
                    public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services, setupProvider.GetSetup);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureName, IStaticConsumer<TRequest, TDependencies>
                    where TSelf : Build<TSelf>, new()
                {
                    protected abstract string FeatureName { get; }
                    protected abstract IEndpoint.Setup Endpoint { get; }
                    protected abstract ConsumerName ConsumerName { get; }
                    protected abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);

                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                    static string IFeatureName.FeatureName => new TSelf().FeatureName;
                    static ConsumerName IStaticConsumer<TRequest, TDependencies>.ConsumerName => new TSelf().ConsumerName;
                    static Task<OneOf<Success, Error>> IStaticConsumer<TRequest, TDependencies>.Consume(TRequest request, TDependencies dependencies) => new TSelf().Consume(request, dependencies);
                }      
            }
        }
    }
}