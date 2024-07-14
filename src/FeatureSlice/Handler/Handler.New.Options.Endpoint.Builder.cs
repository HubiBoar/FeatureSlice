namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options
    {
        public Endpoint.Builder Map(HttpMethod method, string path)
        {
            return new (this, method)
            {
                Path = path
            };
        }

        public Endpoint.Builder MapGet(string path) => Map(HttpMethod.Get, path);
        public Endpoint.Builder MapPost(string path) => Map(HttpMethod.Post, path);

        public sealed partial record Endpoint
        {           
            public sealed partial record Builder(Options Options, HttpMethod Method)
            {
                public required string Path { get; set; }
            }
        }
    }
}