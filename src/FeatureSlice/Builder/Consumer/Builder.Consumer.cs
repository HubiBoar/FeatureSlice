using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;


namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static partial class WithConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
        where TRequest : notnull
    {
        public abstract class Build<TSelf> : ConsumerBase<TSelf, TRequest, TDependencies>
            where TSelf : Build<TSelf>, new()
        {
            public static void Register(IServiceCollection services, Messaging.ISetup setup)
            {
                var self = new TSelf();
                var consumerName = self.ConsumerName;
                var handle = self.Consume;
                var serviceLifetime = self.ServiceLifetime;

                services
                    .FeatureSlice()
                    .WithConsumer<Dispatch, TRequest, TDependencies>(
                        setup,
                        consumerName,
                        (request, dep) =>  handle(request, dep),
                        h => h.Invoke,
                        serviceLifetime);
            }
        }
    }
}