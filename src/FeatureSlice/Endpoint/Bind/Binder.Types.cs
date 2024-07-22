using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace FeatureSlice;

public sealed record FromBodyJsonBinder<T>() : FromBodyBinder<T>("application/json")
    where T : notnull
{
    public override ValueTask<T> BindAsync(HttpContext context)
    {
        return context.Request.ReadFromJsonAsync<T>()!;
    }
}

public abstract record FromBodyBinder<T>(string contentType) : ILastBinder<T>
    where T : notnull
{
    public void ExtendEndpoint(IEndpointBuilder builder)
    {
        builder.Extend(x => x.Accepts<T>(contentType));
    }

    public abstract ValueTask<T> BindAsync(HttpContext context);
}

public sealed record FromRouteBinder<T, TParameter>
(
    string Name,
    bool Required
)
: ParameterBinder<T, TParameter>(Name, Required, ParameterLocation.Path)
    where TParameter : IParameterOpenApiType<T>
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

public sealed record FromQueryBinder<T, TParameter>
(
    string Name,
    bool Required
)
: ParameterBinder<T, TParameter>(Name, Required, ParameterLocation.Query)
    where TParameter : IParameterOpenApiType<T>
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.Query[Name];

        return JsonSerializer.Deserialize<T>(value!)!;
    }
}

public sealed record FromHeaderBinder<T, TParameter>
(
    string Name,
    bool Required
)
: ParameterBinder<T, TParameter>(Name, Required, ParameterLocation.Header)
    where TParameter : IParameterOpenApiType<T>
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.Headers[Name]!;

        return JsonSerializer.Deserialize<T>(value!)!;
    }
}

public sealed record FromCookieBinder<T, TParameter>
(
    string Name,
    bool Required
)
: ParameterBinder<T, TParameter>(Name, Required, ParameterLocation.Cookie)
    where TParameter : IParameterOpenApiType<T>
{
    protected override T Get(HttpContext context)
    {
        var value = context.Request.Cookies[Name]!;

        return JsonSerializer.Deserialize<T>(value!)!;
    }
}

public abstract record ParameterBinder<T, TParameter>
(
    string Name,
    bool Required,
    ParameterLocation In
)
: IAnyBinder<T>
    where TParameter : IParameterOpenApiType<T>
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
                Schema = TParameter.GetSchema()
            });

            return openApi;
        }));
    }
}