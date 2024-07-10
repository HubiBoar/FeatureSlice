namespace FeatureSlice;

public abstract class Dispatchable<TSelf, TRequest, TResponse>
    where TSelf     : Dispatchable<TSelf, TRequest, TResponse>
    where TRequest  : notnull
    where TResponse : notnull
{
    public delegate Task<TResponse> Dispatch(TRequest request);
}