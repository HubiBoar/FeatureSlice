using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IEndpointBuilder
{
    public HttpMethod Method { get; }
    public string Path { get; set ;}

    public void Extend(Action<RouteHandlerBuilder> builder);
}

public sealed record EndpointMapper(Action<IEndpointRouteBuilder> Map);

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

public static class FeatureSliceEndpointExtensions
{
    public static void MapFeatureSlices(this IEndpointRouteBuilder endpointRoute)
    {
        var services = endpointRoute.ServiceProvider.GetServices<EndpointMapper>();

        foreach(var service in services)
        {
            service.Map(endpointRoute);
        }
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options Map<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        HttpMethod method,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        options.Extend(services => 
        {
            services.AddSingleton(new EndpointMapper(route => 
            {
                var endpoint = new EndpointBuilder<TRequest, TResult, TResponse>
                (
                    method,
                    route,
                    provider => request => provider.GetRequiredService<FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Dispatch>()(request)
                )
                {
                    Path = path
                };

                var handler = builder(endpoint);

                foreach(var extension in endpoint.Extensions)
                {
                    extension(handler);
                }
            }));
        });

        return options;
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapGet<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Get, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapPost<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Post, path, builder);
    }
}