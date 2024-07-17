using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeatureSlice;

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