using Explicit.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class AsFlag
    {
        public static partial class WithHandler<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public abstract class BuildAs<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>
                where TSelf : BuildAs<TSelf>, IFeatureName
            {
                public static void Register(IServiceCollection services)
                {
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IFeatureName
                where TSelf : Build<TSelf>, new ()
            {
                protected abstract string FeatureName { get; }

                static string IFeatureName.FeatureName => new TSelf().FeatureName;
            }        
        }
    }
}