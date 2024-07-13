namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options
    {
        public Endpoint.Builder AddEndpoint(HttpMethod method, string path)
        {
            return new (this, method, path);
        }

        public sealed partial record Endpoint
        {           
            public sealed partial record Builder(Options Options, HttpMethod Method, string Path)
            {
            }
        }
    }
}