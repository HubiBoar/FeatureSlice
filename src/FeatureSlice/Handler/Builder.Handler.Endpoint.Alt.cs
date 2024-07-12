using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Definit.Endpoint;
using Definit.Dependencies;
using Definit.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeatureSlice;

public interface IHttpMethod
{
    public abstract static HttpMethod Method { get; }

    public sealed class Get : IHttpMethod
    {
        public static HttpMethod Method { get; } = HttpMethod.Get; 
    }

    public sealed class Post : IHttpMethod
    {
        public static HttpMethod Method { get; } = HttpMethod.Post; 
    }
    
    public sealed class Delete : IHttpMethod
    {
        public static HttpMethod Method { get; } = HttpMethod.Delete; 
    }
}

public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public static partial class WithEndpoint<TMethod, TResult>
        where TMethod : IHttpMethod
        where TResult : Microsoft.AspNetCore.Http.IResult
    {
        public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, Result<TResponse>, TResponse, TDependencies>, IEndpointProvider
            where TSelf : Build<TSelf>, IEndpointProvider, new()
        {
            static Endpoint IEndpointProvider.Endpoint { get; } = new TSelf().Endpoint;

            protected Endpoint Endpoint => new (builder => builder.MapMethods(Path, [ TMethod.Method.ToString() ], OnEndpoint));

            protected abstract string Path { get; }

            protected abstract TResult MapResult(Result<TResponse> result);

            protected virtual async Task<TResult> OnEndpoint([FromBody] TRequest request, [FromServices] Handle handle)
            {
                var response = await handle(request);

                return MapResult(response);
            }

            public static void Register
            (
                IServiceCollection services,
                IHostExtender<WebApplication> extender
            )
            {
                RegisterHandler(services);
                extender.Map(TSelf.Endpoint);
            }
    
            public static void Register
            (
                IServiceCollection services,
                IHostExtender<WebApplication> extender,
                ServiceFactory<IHandlerSetup> setupFactory 
            )
            {
                RegisterHandler(services, setupFactory);
                extender.Map(TSelf.Endpoint);
            } 
        }
    }

    public static partial class WithEndpoint<TMethod>
        where TMethod : IHttpMethod
    {
        public abstract class Build<TSelf> : WithEndpoint<TMethod, Results<Ok<TResponse>, BadRequest<string>>>.Build<TSelf>
            where TSelf : Build<TSelf>, IEndpointProvider, new()
        {
            protected override Results<Ok<TResponse>, BadRequest<string>> MapResult(Result<TResponse> result)
            {
                if(result.Is(out Error error).Else(out var response))
                {
                    return error.ToBadRequest();
                }
                else
                {
                    return Microsoft.AspNetCore.Http.TypedResults.Ok(response)
                }
            }
        }
    }
}