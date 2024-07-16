using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeatureSlice;

public static class FeatureSliceRequestExtensions
{
    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        ILastBinder<T0> bind0,
        Func<T0, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);

            return await mapRequest(value0);
        },
        bind0.ExtendEndpoint));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        ILastBinder<T0> bind0,
        Func<T0, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        
        return Request(builder, bind0, v0 => Task.FromResult(mapRequest(v0)));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        ILastBinder<T1> bind1,
        Func<T0, T1, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);
            var value1 = await bind1.BindAsync(context);

            return await mapRequest(value0, value1);
        },
        endpoint => 
        {
            bind0.ExtendEndpoint(endpoint);
            bind1.ExtendEndpoint(endpoint);
        }));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        ILastBinder<T1> bind1,
        Func<T0, T1, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        
        return Request(builder, bind0, bind1, (v0, v1) => Task.FromResult(mapRequest(v0, v1)));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        ILastBinder<T2> bind2,
        Func<T0, T1, T2, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);
            var value1 = await bind1.BindAsync(context);
            var value2 = await bind2.BindAsync(context);

            return await mapRequest(value0, value1, value2);
        },
        endpoint => 
        {
            bind0.ExtendEndpoint(endpoint);
            bind1.ExtendEndpoint(endpoint);
            bind2.ExtendEndpoint(endpoint);
        }));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        ILastBinder<T2> bind2,
        Func<T0, T1, T2, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        
        return Request(builder, bind0, bind1, bind2, (v0, v1, v2) => Task.FromResult(mapRequest(v0, v1, v2)));
    }
}

public sealed record RequestBuilder<TRequest>
(
    Func<HttpContext, Task<TRequest>> MapRequest,
    Action<IEndpointBuilder> ExtendEndpoint
)
    where TRequest : notnull;

public sealed record ResponseBuilder<TRequest, TResult, TResponse>
(
    EndpointBuilder<TRequest, TResult, TResponse> Builder,
    RequestBuilder<TRequest> Request
)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public RouteHandlerBuilder Response<THttpResult>(Func<TResult, Task<THttpResult>> mapResult)
        where THttpResult : IResult
    {
        Request.ExtendEndpoint(Builder);

        Delegate onEndpoint = OnEndpoint;

        return Builder.EndpointRouteBuilder.MapMethods(Builder.Path, [Builder.Method.Method], onEndpoint);

        async Task<THttpResult> OnEndpoint(HttpContext context)
        {
            var dispatch = Builder.DispatchFactory(context.RequestServices);
            var request = await Request.MapRequest(context);

            var response = await dispatch(request);

            return await  mapResult(response);
        }
    }

    public RouteHandlerBuilder DefaultResponse()
    {
        return Response(DefaultHttpResultAsync);
    }

    public static Task<Results<Ok<TResponse>, BadRequest<string>>> DefaultHttpResultAsync(TResult result)
    {
        return Task.FromResult(DefaultHttpResult(result));
    }

    public static Results<Ok<TResponse>, BadRequest<string>> DefaultHttpResult(TResult result)
    {
        if(result.Is(out Error error).Else(out var response))
        {
            return error.ToBadRequest();
        }
        else
        {
            return TypedResults.Ok(response);
        }
    }
}