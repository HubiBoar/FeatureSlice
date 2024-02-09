using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithConsumer<TRequest, TConsumer>
        where TConsumer : class, IConsumer<TRequest>
    {
        public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>
            where TSelf : Build<TSelf>
        {
            public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider)
            {
                RegisterBase(services, setupProvider.GetSetup);
            }
        }
    }
}