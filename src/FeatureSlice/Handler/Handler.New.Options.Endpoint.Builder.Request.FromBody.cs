using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options
    {
        public sealed partial record Endpoint
        {           
            public sealed partial record Builder
            {
                public FromBodyRequest<TRequest> FromBody()
                {
                    return FromBody<TRequest>(request => request);
                }

                public FromBodyRequest<T0> FromBody<T0>(Func<T0, Task<TRequest>> mapRequest)
                {
                    return new (this, mapRequest);
                }

                public FromBodyRequest<T0> FromBody<T0>(Func<T0, TRequest> mapRequest)
                {
                    return new (this, t0 => Task.FromResult(mapRequest(t0)));
                }

                public sealed record FromBodyRequest<T0>(Builder Builder, Func<T0, Task<TRequest>> MapRequest)
                { 
                    public Endpoint Result<THttpResult>(Func<TResult, Task<THttpResult>> mapResult)
                        where THttpResult : IResult
                    {
                        return new Endpoint(Builder.Options, builder => builder.MapMethods(Builder.Path, [Builder.Method.Method], OnEndpoint));

                        async Task<THttpResult> OnEndpoint([FromBody] T0 body, [FromServices] Dispatch dispatch)
                        {
                            var request = await MapRequest(body);

                            var response = await dispatch(request);

                            return await  mapResult(response);
                        }
                    }

                    public Endpoint DefaultResult()
                    {
                        return Result(DefaultHttpResultAsync);
                    }
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
        }
    }
}