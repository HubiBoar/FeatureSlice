using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithEndpoint
    {
        public abstract class Build<TSelf> : IEndpointProvider
            where TSelf : Build<TSelf>, new()
        {
            static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

            protected abstract Endpoint Endpoint { get; }

            public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
            {
                var self = new TSelf();
                var endpoint = self.Endpoint;

                services
                    .FeatureSlice()
                    .WithEndpoint(extender, endpoint);
            }
        }
    }
}