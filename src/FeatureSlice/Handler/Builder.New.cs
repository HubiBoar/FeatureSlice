using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FeatureSlice;

public abstract class FeatureSlice2<TSelf, TRequest, TResponse> : FeatureSliceBase<TSelf, TRequest, Result<TResponse>, TResponse>
    where TSelf : FeatureSlice2<TSelf, TRequest, TResponse>, new()
    where TRequest : notnull
    where TResponse : notnull
{
}

public abstract class FeatureSlice2<TSelf, TRequest> : FeatureSliceBase<TSelf, TRequest, Result, Success>
    where TSelf : FeatureSlice2<TSelf, TRequest>, new()
    where TRequest : notnull
{
}

internal interface IEndpointBuilder : IEndpointConventionBuilder
{
    public void Map(IEndpointRouteBuilder builder);
}

public abstract class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
    where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public sealed record Endpoint(Options Options, Func<IEndpointRouteBuilder, IEndpointConventionBuilder> Extender) : IEndpointBuilder
    {
        private readonly List<Action<EndpointBuilder>> _conventions = [];

        public void Add(Action<EndpointBuilder> convention)
        {
            _conventions.Add(convention);
        }

        void IEndpointBuilder.Map(IEndpointRouteBuilder builder)
        {
            var endpointConventionBuilder = Extender(builder);
            foreach (var convention in _conventions)
            {
                endpointConventionBuilder.Add(convention);
            }
        }

        public static implicit operator Options(Endpoint endpoint) => endpoint.Options;
    }

    public sealed record Options(Func<IServiceProvider, Dispatch> DispatchFactory)
    {
        public void Extend(Action<IServiceCollection> extension)
        {
        }

        public EndpointBuilder AddEndpoint
        (
            HttpMethod method,
            string path
        )
        {
            return new 
            (
                this,
                method,
                path
            );
        }

        public void Register
        (
            IServiceCollection services,
            Func<IServiceProvider, Dispatch, Dispatch> dispatcher,
            ServiceLifetime serviceLifetime
        )
        {

        }
    }

    public sealed record EndpointBuilder(Options Options, HttpMethod Method, string Path)
    {
        public sealed record RequestBuilder<T0>(EndpointBuilder Builder, Func<T0, Task<TRequest>> MapRequest)
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
    }
 
    public static Options Handle<TDep0>(Func<TRequest, TDep0, Task<TResult>> func)
    {

    }

    public static Options Handle<TDep0, TDep1>(Func<TRequest, TDep0, TDep1, Task<TResult>> func)
    {

    }

    public delegate Task<TResult> Dispatch(TRequest request);

    public abstract Options Setup { get; }

    public static void Register
    (
        IServiceCollection services
    )
    {

    }
}
