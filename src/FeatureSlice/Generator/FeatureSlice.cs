namespace FeatureSlice.Generated;

public interface IFeatureSlice<TRequest, TResponse>
{

}

public sealed class Sample : IFeatureSlice<Request, Response>
{
    public sealed record Request();

    public sealed record Response();

}