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
                public Endpoint Setup<T0, THttpResult>(Func<T0, TRequest> mapRequest, Func<TResult, THttpResult> mapResult)
                    where THttpResult : IResult
                {
                    return new Endpoint(Options, builder => builder.MapMethods(Path, [Method.Method], OnEndpoint));

                    async Task<THttpResult> OnEndpoint(T0 t0, [FromServices] Dispatch dispatch)
                    {
                        var request = mapRequest(t0);

                        var response = await dispatch(request);

                        return mapResult(response);
                    }
                }

                public Endpoint Setup<T0, T1, THttpResult>(Func<T0, T1, TRequest> mapRequest, Func<TResult, THttpResult> mapResult)
                    where THttpResult : IResult
                {
                    return new Endpoint(Options, builder => builder.MapMethods(Path, [Method.Method], OnEndpoint));

                    async Task<THttpResult> OnEndpoint(T0 t0, T1 t1, [FromServices] Dispatch dispatch)
                    {
                        var request = mapRequest(t0, t1);

                        var response = await dispatch(request);

                        return mapResult(response);
                    }
                }

                public Endpoint Setup<T0>(Func<T0, TRequest> mapRequest)
                {
                    return Setup(mapRequest, DefaultHttpResult);
                }

                public Endpoint Setup<T0, T1>(Func<T0, T1, TRequest> mapRequest)
                {
                    return Setup(mapRequest, DefaultHttpResult);
                }

                public RequestBuilder Request()
                {
                    return new (this);
                }

                public sealed record RequestBuilder(Builder Builder)
                {
                    public FromRouteBuilder<TRoute> FromRoute<TRoute>()
                    {
                        return new FromRouteBuilder<TRoute>(Builder);
                    }

                    public sealed record FromRouteBuilder<TRoute>(Builder Builder)
                    {
                        public Endpoint Result<THttpResult>(Func<TRoute, Task<TRequest>> mapRequest, Func<TResult, Task<THttpResult>> mapResult)
                            where THttpResult : IResult
                        {
                            var builder = Builder;
                            return new Endpoint(builder.Options, x => x.MapMethods(builder.Path, [builder.Method.Method], OnEndpoint));

                            async Task<THttpResult> OnEndpoint([FromRoute] TRoute id, [FromServices] Dispatch dispatch)
                            {
                                var request = await mapRequest(id);

                                var response = await dispatch(request);

                                return await  mapResult(response);
                            }
                        }

                        public Endpoint DefaultResultAsync(Func<TRoute, Task<TRequest>> mapRequest)
                        {
                            return Result(mapRequest, DefaultHttpResultAsync);
                        }

                        public Endpoint DefaultResult(Func<TRoute, TRequest> mapRequest)
                        {
                            return DefaultResultAsync(route => Task.FromResult(mapRequest(route)));
                        }
                    }
                }
            }
        }
    }
}