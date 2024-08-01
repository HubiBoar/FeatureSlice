using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class FeatureSliceEndpointExtensions
{
    public static IFeatureSliceSetup<TRequest, TResult, TResponse> Map<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        HttpMethod method,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        options.Extend(services => 
        {
            services.AddSwaggerGen(options => options.SetCustomSchemaId());

            services.AddFeatureSlicesExtension<WebApplication>((host, provider) => 
            {
                var endpoint = new EndpointBuilder<TRequest, TResult, TResponse>
                (
                    method,
                    host,
                    provider => request => options.GetDispatch(provider)(request)
                )
                {
                    Path = path
                };

                var handler = builder(endpoint);

                foreach(var extension in endpoint.Extensions)
                {
                    extension(handler);
                }

                return Task.CompletedTask;
            });
        });

        return options;
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapGet<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Get, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapPost<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Post, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapPut<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Put, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapPatch<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Patch, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse>MapOptions<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse>options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Options, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse>MapHead<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse>options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Head, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse>MapTrace<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse>options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Trace, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse>MapDelete<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse>options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Delete, path, builder);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse>MapConnect<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse>options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Connect, path, builder);
    }
}