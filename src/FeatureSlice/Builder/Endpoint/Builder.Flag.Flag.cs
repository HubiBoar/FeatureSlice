using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithFlag
    {
        public static partial class WithEndpoint
        {
            public abstract class Build<TSelf> : IEndpoint
                where TSelf : Build<TSelf>, new()
            {
                static IEndpoint.Setup IEndpoint.Endpoint { get; } = new TSelf().Endpoint;

                protected abstract string FeatureName { get; }
                protected abstract IEndpoint.Setup Endpoint { get; }

                public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
                {
                    var self = new TSelf();
                    var endpoint = self.Endpoint;
                    var featureName = self.FeatureName;

                    services
                        .FeatureSlice()
                        .WithFlag(featureName)
                        .WithEndpoint(extender, endpoint);
                }
            }
        }
    }
}