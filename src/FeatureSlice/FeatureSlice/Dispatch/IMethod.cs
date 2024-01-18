namespace FeatureSlice.Dispatch;

public interface IMethod<TRequest, TResponse>
{
    public TResponse Handle(TRequest request);
}