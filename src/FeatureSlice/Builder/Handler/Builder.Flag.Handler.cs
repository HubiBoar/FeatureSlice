using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithFlag
    {
        public static partial class WithHandler<TRequest, TResponse, TDependencies>
            where TDependencies : class, IFromServices<TDependencies>
            where TRequest : notnull
            where TResponse : notnull
        {
            public abstract class Build<TSelf> : HandlerBaseWithFlag<TSelf, TRequest, TResponse, TDependencies>
                where TSelf : Build<TSelf>, new()
            {
                public static void Register(IServiceCollection services)
                {
                    var self = new TSelf();
                    var featureName = self.FeatureName;
                    var handle = self.Handle;
                    var serviceLifetime = self.ServiceLifetime;

                    services
                        .FeatureSlice()
                        .WithFlag(featureName)
                        .WithHandler<Dispatch, TRequest, TResponse, TDependencies>(
                            (request, dep) => handle(request, dep),
                            h => h.Invoke,
                            serviceLifetime);
                }
            }
        }
    }
}