using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace FeatureSlice;

public interface IRouteName
{
    public static abstract string Name { get; }

    public sealed record Id : IRouteName
    {
        public static string Name => "id";
    }
}

public sealed record FromRouteBinder<T, TName>(T Value) : IBindable<FromRouteBinder<T, TName>>
    where TName : IRouteName
{
    public static ValueTask<FromRouteBinder<T, TName>> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var name = TName.Name;
        var value = (T)context.Request.RouteValues[name!]!;

        return ValueTask.FromResult(new FromRouteBinder<T, TName>(value));
    }

    public static void ExtendEndpoint(IEndpointConventionBuilder builder)
    {
        builder.WithOpenApi(openApi =>
        {
            openApi.Parameters.Add(new ());

            return openApi;
        });
    }
}


public interface IBindable<TSelf>
    where TSelf : IBindable<TSelf>
{
    public abstract static ValueTask<TSelf> BindAsync(HttpContext context, ParameterInfo parameter);

    public abstract static void ExtendEndpoint(IEndpointConventionBuilder builder);
}

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
                    var parameters = mapRequest.Method.GetParameters();
                    var attributes = parameters.SelectMany(x => x.GetCustomAttributesData()).ToArray();
                    foreach(var attribute in attributes)
                    {
                        Console.WriteLine(attribute.AttributeType);
                    }

                    var metadata = RequestDelegateFactory.Create(OnEndpoint);

                    return new Endpoint(Options, builder => builder.MapMethods(Path, [Method.Method], OnEndpoint))
                        .WithFormOptions();

                    async Task<THttpResult> OnEndpoint(T0 t0, [FromBody] T1 t1, [FromServices] Dispatch dispatch)
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
                    public FromRouteBuilder<TRoute, TName> FromRoute<TRoute, TName>()
                        where TName : IRouteName
                    {
                        Builder.Path += "/{" + TName.Name + "}";
                        return new FromRouteBuilder<TRoute, TName>(Builder);
                    }

                    public sealed record FromRouteBuilder<TRoute, TName>(Builder Builder)
                        where TName : IRouteName
                    {
                        public Endpoint Result<THttpResult>(Func<TRoute, Task<TRequest>> mapRequest, Func<TResult, Task<THttpResult>> mapResult)
                            where THttpResult : IResult
                        {
                            var builder = Builder;
                            var metadata = RequestDelegateFactory.Create(OnEndpoint);

                            var endpoint = new Endpoint(builder.Options, x => x.MapMethods(builder.Path, [builder.Method.Method], OnEndpoint));

                            FromRouteBinder<TRoute, TName>.ExtendEndpoint(endpoint);

                            return endpoint!;

                            async Task<THttpResult> OnEndpoint(FromRouteBinder<TRoute, TName> route, [FromServices] Dispatch dispatch)
                            {
                                var request = await mapRequest(route.Value);

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