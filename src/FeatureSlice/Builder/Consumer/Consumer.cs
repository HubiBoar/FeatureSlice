using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public abstract class ConsumerBase<TSelf, TRequest, TDependencies> : DispatchableWithFlag<TSelf, TRequest, Success>
    where TSelf : ConsumerBase<TSelf, TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract ConsumerName ConsumerName { get; }

    protected abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);
}

public abstract class ConsumerBaseWithFlag<TSelf, TRequest, TDependencies> : ConsumerBase<TSelf, TRequest, TDependencies>
    where TSelf : ConsumerBaseWithFlag<TSelf, TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    protected virtual string FeatureName => ConsumerName.Name;
}
