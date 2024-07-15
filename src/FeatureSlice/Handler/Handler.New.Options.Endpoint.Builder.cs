using Microsoft.AspNetCore.Builder;

namespace FeatureSlice;

public interface IEndpointBuilder
{
    public HttpMethod Method { get; }
    public string Path { get; set ;}

    public void Extend(Action<IEndpointConventionBuilder> builder);
}

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
            public sealed partial record Builder(Options Options, HttpMethod Method) : IEndpointBuilder
            {
                public required string Path { get; set; }

                private readonly List<Action<IEndpointConventionBuilder>> _extensions = [];

                public void Extend(Action<IEndpointConventionBuilder> builder)
                {
                    _extensions.Add(builder);
                }

                public void Extend(IEndpointConventionBuilder builder)
                {
                    foreach(var extension in _extensions)
                    {
                        extension(builder);
                    }
                } 
            }
        }
    }
}