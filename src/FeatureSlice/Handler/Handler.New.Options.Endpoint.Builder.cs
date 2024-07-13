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
            public sealed partial record Builder(Options Options, HttpMethod Method, string Path)
            {
                public RequestBuilder<TRequest> RequestFromBody()
                {
                    return FromBody<TRequest>(request => request);
                }

                public RequestBuilder<T0> FromBody<T0>(Func<T0, Task<TRequest>> mapRequest)
                {
                    return new (this, mapRequest);
                }

                public RequestBuilder<T0> FromBody<T0>(Func<T0, TRequest> mapRequest)
                {
                    return new (this, t0 => Task.FromResult(mapRequest(t0)));
                }

                public sealed record RequestBuilder<T0>(Builder Builder, Func<T0, Task<TRequest>> MapRequest)
                { 
                    public Endpoint WithResult<THttpResult>(Func<TResult, THttpResult> mapResult)
                        where THttpResult : IResult
                    {
                        return new Endpoint(Builder.Options, builder => builder.MapMethods(Builder.Path, [Builder.Method.Method], OnEndpoint));

                        async Task<THttpResult> OnEndpoint(T0 t0, [FromServices] Dispatch dispatch)
                        {
                            var request = await MapRequest(t0);

                            var response = await dispatch(request);

                            return mapResult(response);
                        }
                    }

                    public Endpoint WithDefaultResult()
                    {
                        return WithResult<Results<Ok<TResponse>, BadRequest<string>>>(result =>
                        {
                            if(result.Is(out Error error).Else(out var response))
                            {
                                return error.ToBadRequest();
                            }
                            else
                            {
                                return TypedResults.Ok(response);
                            }
                        });
                    }
                }
            }
 
        }
    }
}