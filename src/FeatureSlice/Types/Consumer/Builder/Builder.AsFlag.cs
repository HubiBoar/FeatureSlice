using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsFlag
    {
        public static class WithConsumer<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public abstract class BuildAs<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>
                where TSelf : BuildAs<TSelf>, IFeatureFlag
            {
                public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider)
                {
                    RegisterBase(services, setupProvider.GetSetup);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IFeatureFlag
                where TSelf : Build<TSelf>, new()
            {
                protected abstract string FeatureName { get; }
                static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
            }       
        }
    }
}