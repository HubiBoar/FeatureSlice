using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
    public static class AsConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        public abstract class BuildAs<TSelf> : StaticConsumerFeatureSlice.Default<TRequest, TSelf, TDependencies>
            where TSelf : BuildAs<TSelf>, IStaticConsumer<TRequest, TDependencies>
        {
            public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider)
            {
                RegisterBase(services, setupProvider.GetSetup);
            }
        }

        public abstract class Build<TSelf> : BuildAs<TSelf>, IStaticConsumer<TRequest, TDependencies>
            where TSelf : Build<TSelf>, new()
        {
            protected abstract ConsumerName ConsumerName { get; }
            protected abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);

            static ConsumerName IStaticConsumer<TRequest, TDependencies>.ConsumerName => new TSelf().ConsumerName;
            static Task<OneOf<Success, Error>> IStaticConsumer<TRequest, TDependencies>.Consume(TRequest request, TDependencies dependencies) => new TSelf().Consume(request, dependencies);
        }
    }
}