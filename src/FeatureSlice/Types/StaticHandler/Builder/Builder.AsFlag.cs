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
        public static class AsHandler<TRequest, TResponse, TDependencies>
            where TDependencies : class, IFromServices<TDependencies>
        {
            public abstract class BuildAs<TSelf> : StaticHandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, TSelf, TDependencies>
                where TSelf : BuildAs<TSelf>, IFeatureName, IStaticHandler<TRequest, TResponse, TDependencies>
            {
                public static void Register(IServiceCollection services)
                {
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IFeatureName, IStaticHandler<TRequest, TResponse, TDependencies>
                where TSelf : Build<TSelf>, new()
            {
                protected abstract string FeatureName { get; }
                protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);

                static string IFeatureName.FeatureName => new TSelf().FeatureName;
                static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies) => new TSelf().Handle(request, dependencies);
            }    
        }
    }
}