using System.Collections;
using Microsoft.AspNetCore.Http;
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