using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class Consumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
        where TRequest : notnull
    {
        public static partial class WithFlag
        {
            public abstract class Build<TSelf> : ConsumerBaseWithFlag<TSelf, TRequest, TDependencies>
                where TSelf : Build<TSelf>, new()
            {
                public static void Register(IServiceCollection services, Messaging.ISetup setup)
                {
                    var self = new TSelf();
                    var featureName = self.FeatureName;
                    var consumerName = self.ConsumerName;
                    var handle = self.Consume;
                    var serviceLifetime = self.ServiceLifetime;

                    services
                        .FeatureSlice()
                        .WithFlag(featureName)
                        .WithConsumer<Dispatch, TRequest, TDependencies>(
                            setup,
                            consumerName,
                            (request, dep) => handle(request, dep),
                            h => h.Invoke,
                            serviceLifetime);
                }
            }
        }
    }
}