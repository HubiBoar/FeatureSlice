using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace FeatureSlice;

//https://andrewlock.net/behind-the-scenes-of-minimal-apis-3-exploring-the-model-binding-logic-of-minimal-apis/
internal static class OpenApiSchemaGenerator
{
    private static readonly Dictionary<Type, (string, string?)> simpleTypesAndFormats =
        new()
        {
            [typeof(bool)] = ("boolean", null),
            [typeof(byte)] = ("string", "byte"),
            [typeof(int)] = ("integer", "int32"),
            [typeof(uint)] = ("integer", "int32"),
            [typeof(ushort)] = ("integer", "int32"),
            [typeof(long)] = ("integer", "int64"),
            [typeof(ulong)] = ("integer", "int64"),
            [typeof(float)] = ("number", "float"),
            [typeof(double)] = ("number", "double"),
            [typeof(decimal)] = ("number", "double"),
            [typeof(DateTime)] = ("string", "date-time"),
            [typeof(DateTimeOffset)] = ("string", "date-time"),
            [typeof(TimeSpan)] = ("string", "date-span"),
            [typeof(Guid)] = ("string", "uuid"),
            [typeof(char)] = ("string", null),
            [typeof(Uri)] = ("string", "uri"),
            [typeof(string)] = ("string", null),
            [typeof(object)] = ("object", null)
        };

    internal static OpenApiSchema GetOpenApiSchema<T>()
    {
        var type = typeof(T);
        if (type is null)
        {
            return new OpenApiSchema();
        }

        var (openApiType, openApiFormat) = GetTypeAndFormatProperties(type);
        return new OpenApiSchema
        {
            Type = openApiType,
            Format = openApiFormat,
            Nullable = Nullable.GetUnderlyingType(type) != null,
        };
    }

    private static (string, string?) GetTypeAndFormatProperties(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (simpleTypesAndFormats.TryGetValue(type, out var typeAndFormat))
        {
            return typeAndFormat;
        }

        if (type == typeof(IFormFileCollection) || type == typeof(IFormFile))
        {
            return ("object", null);
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            return ("object", null);
        }

        if (type != typeof(string) && (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)))
        {
            return ("array", null);
        }

        return ("object", null);
    }
}


public sealed record FromBodyJsonBinder<T>() : FromBodyBinder<T>("application/json")
    where T : notnull;

public abstract record FromBodyBinder<T>(string contentType) : ILastBinder<T>
    where T : notnull
{
    public ValueTask<T> BindAsync(HttpContext context)
    {
        return context.Request.ReadFromJsonAsync<T>()!;
    }

    public void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Extend(x => x.Accepts<T>(contentType));
    }
}

public sealed record FromRouteBinder<T>
(
    string Name,
    bool Required
)
    : ParameterBinder<T>(Name, Required, ParameterLocation.Query)
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.RouteValues[Name]!;

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public override void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Path += "/{" + Name + "}";

        base.ExtendEndpoint(builder);
    }
}
 
public sealed record FromQueryBinder<T>
(
    string Name,
    bool Required
)
    : ParameterBinder<T>(Name, Required, ParameterLocation.Query)
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.Query[Name]!;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}

public sealed record FromHeaderBinder<T>
(
    string Name,
    bool Required
)
    : ParameterBinder<T>(Name, Required, ParameterLocation.Header)
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.Headers[Name]!;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}

public sealed record FromCookieBinder<T>
(
    string Name,
    bool Required
)
    : ParameterBinder<T>(Name, Required, ParameterLocation.Cookie)
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.Cookies[Name]!;

        return (T)Convert.ChangeType(value, typeof(T));
    }
}

public abstract record ParameterBinder<T>
(
    string Name,
    bool Required,
    ParameterLocation In
)
    : IAnyBinder<T>
{
    protected abstract T Get(HttpContext context);

    public ValueTask<T> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(Get(context));
    }

    public virtual void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Extend(x => x.WithOpenApi(openApi =>
        {
            openApi.Parameters.Add(new OpenApiParameter()
            {
                Name = Name,
                In = In,
                Required = Required,
                Schema = OpenApiSchemaGenerator.GetOpenApiSchema<T>()
            });

            return openApi;
        }));
    }
}


public static class Binder
{
    public static FromBodyJsonBinder<T> FromBodyJson<T>()
        where T : notnull
    {
        return new ();
    }

    public static FromQueryBinder<T> FromQuery<T>(string name, bool required = true)
    {
        return new (name, required);
    }

    public static FromRouteBinder<T> FromRoute<T>(string name, bool required = true)
    {
        return new (name, required);
    }

    public static FromHeaderBinder<T> FromHeader<T>(string name, bool required = true)
    {
        return new (name, required);
    }

    public static FromCookieBinder<T> FromCookie<T>(string name, bool required = true)
    {
        return new (name, required);
    }
}

public interface IAnyBinder<T> : ILastBinder<T>
{
}

public interface ILastBinder<T> : IBinder<T>
{
}

public interface IBinder<T>
{
    ValueTask<T> BindAsync(HttpContext context);

    void ExtendEndpoint(IEndpointBuilder builder);
}

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