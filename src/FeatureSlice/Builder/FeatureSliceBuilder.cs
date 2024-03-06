using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class FeatureSliceBuilder
{
}

public abstract class Dispatchable<TSelf, TRequest, TResponse>
    where TSelf: Dispatchable<TSelf, TRequest, TResponse>
    where TRequest : notnull
{
    public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);
}

public abstract class DispatchableWithFlag<TSelf, TRequest, TResponse>
    where TSelf: DispatchableWithFlag<TSelf, TRequest, TResponse>
    where TRequest : notnull
{
    public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);
}