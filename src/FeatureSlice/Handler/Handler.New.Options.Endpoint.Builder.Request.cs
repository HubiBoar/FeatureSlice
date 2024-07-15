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

public sealed record FromRouteBinder<T>(string Name) : IBindable<T>
{
    public ValueTask<T> BindAsync(HttpContext context)
    {
        var value = context.Request.RouteValues[Name]!;

        //var deserialized = JsonSerializer.Deserialize<T>(value);
        T converted = (T)Convert.ChangeType(value, typeof(T));

        return ValueTask.FromResult(converted);
    }

    public void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Path += "/{" + Name + "}";

        builder.Extend(x => x.WithOpenApi(openApi =>
        {
            openApi.Parameters.Add(new OpenApiParameter()
            {
                Name = Name,
                In = ParameterLocation.Path,
                Description = "The ID of the item to retrieve",
                Required = true,
                Schema = OpenApiSchemaGenerator.GetOpenApiSchema<T>()
            });

            return openApi;
        }));
    }
}

public interface IBindable<T>
{
    ValueTask<T> BindAsync(HttpContext context);

    void ExtendEndpoint(IEndpointBuilder builder);
}

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options
    {
        public sealed partial record Endpoint
        {           
            public sealed partial record Builder
            {
                public RequestBuilder Request()
                {
                    return new (this);
                }

                public sealed record RequestBuilder(Builder Builder)
                {
                    public FromRouteBuilder<TRoute> FromRoute<TRoute>(string name)
                    {
                        return new FromRouteBuilder<TRoute>(Builder, name);
                    }

                    public sealed record FromRouteBuilder<TRoute>(Builder Builder, string Name)
                    {
                        public Endpoint Result<THttpResult>(Func<TRoute, Task<TRequest>> mapRequest, Func<TResult, Task<THttpResult>> mapResult)
                            where THttpResult : IResult
                        {
                            var builder = Builder;
                            var binder = new FromRouteBinder<TRoute>(Name);
                            var metadata = RequestDelegateFactory.Create(OnEndpoint);

                            binder.ExtendEndpoint(builder);

                            Delegate onEndpoint = OnEndpoint;

                            var endpoint = new Endpoint(builder.Options, x => x.MapMethods(builder.Path, [builder.Method.Method], onEndpoint));

                            builder.Extend(endpoint);

                            return endpoint!;

                            async Task<THttpResult> OnEndpoint(HttpContext context)
                            {
                                var dispatch = context.RequestServices.GetRequiredService<Dispatch>();
                                var route = await binder.BindAsync(context);
                                var request = await mapRequest(route);

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