using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

public sealed record FromRouteIntBinder(string Name) : IManyBinder<int>
{
    public ValueTask<int> BindAsync(HttpContext context)
    {
        var value = context.Request.RouteValues[Name]!.ToString();

        var converted = int.Parse(value!);

        return ValueTask.FromResult(converted);
    }

    public void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Path += "/{" + Name + ":int}";

        builder.Extend(x => x.WithOpenApi(openApi =>
        {
            openApi.Parameters.Add(new OpenApiParameter()
            {
                Name = Name,
                In = ParameterLocation.Path,
                Description = $"The {Name} of the item to retrieve",
                Required = true,
                Schema = OpenApiSchemaGenerator.GetOpenApiSchema<int>()
            });

            return openApi;
        }));
    }
}


public sealed record FromBodyBinder<T>() : ISingleBinder<T, IBindingSubject.Body>
{
    public ValueTask<T> BindAsync(HttpContext context)
    {
        return context.Request.ReadFromJsonAsync<T>()!;
    }

    public void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Extend(x => x.WithOpenApi(openApi =>
        {
            openApi.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["username"] = new OpenApiSchema { Type = "string" },
                                ["email"] = new OpenApiSchema { Type = "string" },
                                ["password"] = new OpenApiSchema { Type = "string" }
                            },
                            Required = new HashSet<string> { "username", "email", "password" }
                        }
                    }
                }
            };
            return openApi;
        }));
    }
}

public static class Binder
{
    public static FromBodyBinder<T> FromBody<T>()
    {
        return new FromBodyBinder<T>();
    }

    public static FromRouteIntBinder FromRouteInt(string name)
    {
        return new FromRouteIntBinder(name);
    }
}


public interface IBindingSubject
{
    public class Body : IBindingSubject;
}

public interface ISingleBinder<T, TSubject> : IBinder<T>
    where TSubject : IBindingSubject
{
}

public interface IManyBinder<T> : IBinder<T>
{
}

public interface IBinder<T>
{
    ValueTask<T> BindAsync(HttpContext context);

    void ExtendEndpoint(IEndpointBuilder builder);
}

public sealed record RequestBuilder<TRequest>
(
    Func<HttpContext, Task<TRequest>> MapRequest,
    Action<IEndpointBuilder> ExtendEndpoint
)
    where TRequest : notnull;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options
    {
        public sealed partial record Endpoint
        {           
            public sealed partial record Builder
            {
                public ResponseBuilder Request<T0>(IBinder<T0> bind0, Func<T0, Task<TRequest>> mapRequest)
                {
                    return new (this, new (async context => 
                    {
                        var value0 = await bind0.BindAsync(context);

                        return await mapRequest(value0);
                    },
                    bind0.ExtendEndpoint));
                }

                public ResponseBuilder Request<T0>(IBinder<T0> bind0, Func<T0, TRequest> mapRequest)
                {
                    return Request(bind0, v0 => Task.FromResult(mapRequest(v0)));
                }

                public ResponseBuilder Request<T0, T1, TSubject>(IManyBinder<T0> bind0, ISingleBinder<T1, TSubject> bind1, Func<T0, T1, Task<TRequest>> mapRequest)
                    where TSubject : IBindingSubject
                {
                    return new (this, new (async context => 
                    {
                        var value0 = await bind0.BindAsync(context);
                        var value1 = await bind1.BindAsync(context);

                        return await mapRequest(value0, value1);
                    },
                    bind0.ExtendEndpoint));
                }

                public ResponseBuilder Request<T0, T1, TSubject>(IManyBinder<T0> bind0, ISingleBinder<T1, TSubject> bind1, Func<T0, T1, TRequest> mapRequest)
                    where TSubject : IBindingSubject
                {
                    return Request(bind0, bind1, (v0, v1) => Task.FromResult(mapRequest(v0, v1)));
                }

                public ResponseBuilder Request<T0, T1>(IManyBinder<T0> bind0, IManyBinder<T1> bind1, Func<T0, T1, Task<TRequest>> mapRequest)
                {
                    return new (this, new (async context => 
                    {
                        var value0 = await bind0.BindAsync(context);
                        var value1 = await bind1.BindAsync(context);

                        return await mapRequest(value0, value1);
                    },
                    bind0.ExtendEndpoint));
                }

                public ResponseBuilder Request<T0, T1>(IManyBinder<T0> bind0, IManyBinder<T1> bind1, Func<T0, T1, TRequest> mapRequest)
                {
                    return Request(bind0, bind1, (v0, v1) => Task.FromResult(mapRequest(v0, v1)));
                }

                public sealed record ResponseBuilder(Builder Builder, RequestBuilder<TRequest> Request)
                {
                    public Endpoint Response<THttpResult>(Func<TResult, Task<THttpResult>> mapResult)
                        where THttpResult : IResult
                    {
                        Request.ExtendEndpoint(Builder);

                        Delegate onEndpoint = OnEndpoint;
                
                        var endpoint = new Endpoint(Builder.Options, x => x.MapMethods(Builder.Path, [Builder.Method.Method], onEndpoint));

                        Builder.Extend(endpoint);

                        return endpoint!;

                        async Task<THttpResult> OnEndpoint(HttpContext context)
                        {
                            var dispatch = context.RequestServices.GetRequiredService<Dispatch>();
                            var request = await Request.MapRequest(context);

                            var response = await dispatch(request);

                            return await  mapResult(response);
                        }
                    }

                    public Endpoint DefaultResponse()
                    {
                        return Response(DefaultHttpResultAsync);
                    }
                }
            }
        }
    }
}