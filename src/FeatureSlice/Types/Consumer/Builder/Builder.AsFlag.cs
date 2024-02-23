using Definit.Configuration;
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
                where TSelf : BuildAs<TSelf>, IFeatureName
            {
                public static void Register(IServiceCollection services, Messaging.ISetup setup)
                {
                    RegisterBase(services, setup);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IFeatureName
                where TSelf : Build<TSelf>, new()
            {
                protected abstract string FeatureName { get; }
                static string IFeatureName.FeatureName => new TSelf().FeatureName;
            }       
        }
    }
}