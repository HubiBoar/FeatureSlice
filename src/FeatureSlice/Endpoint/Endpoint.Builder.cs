using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FeatureSlice;

public interface IEndpointBuilder
{
    public HttpMethod Method { get; }
    public string Path { get; set ;}

    public void Extend(Action<RouteHandlerBuilder> builder);
}

public sealed record EndpointBuilder<TRequest, TResult, TResponse>
(
    HttpMethod Method,
    IEndpointRouteBuilder EndpointRouteBuilder,
    Func<IServiceProvider, Func<TRequest, Task<TResult>>> DispatchFactory
)
: IEndpointBuilder
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public required string Path { get; set; }

    public IReadOnlyCollection<Action<RouteHandlerBuilder>> Extensions => _extensions;
    private readonly List<Action<RouteHandlerBuilder>> _extensions = [];

    public void Extend(Action<RouteHandlerBuilder> builder)
    {
        _extensions.Add(builder);
    }
}