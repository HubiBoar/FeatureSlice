using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace FeatureSlice;

public interface IOpenApiType<TType>
{
    public abstract static OpenApiSchema GetSchema();
}

public interface IParameterOpenApiType<TType> : IOpenApiType<TType>
{
}

public static class OpenApiType
{
    public static class Parameter
    {
        public static OpenApiSchema Schema(string type, string? format, bool nullable) => new ()
        {
            Type = type,
            Format = format,
            Nullable = nullable,
        };

        public static OpenApiSchema GetBoolean(bool nullable)  => Schema("boolean", null, nullable);
        public static OpenApiSchema GetString(bool nullable)   => Schema("string", null, nullable);
        public static OpenApiSchema GetByte(bool nullable)     => Schema("string", "byte", nullable);
        public static OpenApiSchema GetInt32(bool nullable)    => Schema("integer", "int32", nullable);
        public static OpenApiSchema GetInt64(bool nullable)    => Schema("integer", "int64", nullable);
        public static OpenApiSchema GetFloat(bool nullable)    => Schema("number", "float", nullable);
        public static OpenApiSchema GetDouble(bool nullable)   => Schema("number", "double", nullable);
        public static OpenApiSchema GetDecimal(bool nullable)  => Schema("number", "double", nullable);
        public static OpenApiSchema GetDateTime(bool nullable) => Schema("string", "date-time", nullable);
        public static OpenApiSchema GetDateSpan(bool nullable) => Schema("string", "date-span", nullable);
        public static OpenApiSchema GetGuid(bool nullable)     => Schema("string", "uuid", nullable);
        public static OpenApiSchema GetChar(bool nullable)     => Schema("string", null, nullable);
        public static OpenApiSchema GetUri(bool nullable)      => Schema("string", "uri", nullable);
        public static OpenApiSchema GetObject(bool nullable)   => Schema("object", null, nullable);
        public static OpenApiSchema GetArray(bool nullable)    => Schema("array", null, nullable);


        public sealed record Bool : IParameterOpenApiType<bool>
        {
            public static OpenApiSchema GetSchema() => GetBoolean(false);

            public sealed record Nullable : IParameterOpenApiType<bool?>
            {
                public static OpenApiSchema GetSchema() => GetBoolean(true);
            }
        }

        public sealed record Byte : IParameterOpenApiType<byte>
        {
            public static OpenApiSchema GetSchema() => GetByte(false);

            public sealed record Nullable : IParameterOpenApiType<byte?>
            {
                public static OpenApiSchema GetSchema() => GetByte(true);
            }
        }

        public sealed record Int : IParameterOpenApiType<int>
        {
            public static OpenApiSchema GetSchema() => GetInt32(false);

            public sealed record Nullable : IParameterOpenApiType<int?>
            {
                public static OpenApiSchema GetSchema() => GetInt32(true);
            } 
        }

        public sealed record UInt : IParameterOpenApiType<uint>
        {
            public static OpenApiSchema GetSchema() => GetInt32(false);

            public sealed record Nullable : IParameterOpenApiType<uint?>
            {
                public static OpenApiSchema GetSchema() => GetInt32(true);
            }
        }

        public sealed record UShort : IParameterOpenApiType<ushort>
        {
            public static OpenApiSchema GetSchema() => GetInt32(false);

            public sealed record Nullable : IParameterOpenApiType<ushort?>
            {
                public static OpenApiSchema GetSchema() => GetInt32(true);
            }
        }

        public sealed record Long : IParameterOpenApiType<long>
        {
            public static OpenApiSchema GetSchema() => GetInt64(false);

            public sealed record Nullable : IParameterOpenApiType<long?>
            {
                public static OpenApiSchema GetSchema() => GetInt64(true);
            }
        }

        public sealed record ULong : IParameterOpenApiType<ulong>
        {
            public static OpenApiSchema GetSchema() => GetInt64(false);

            public sealed record Nullable : IParameterOpenApiType<ulong?>
            {
                public static OpenApiSchema GetSchema() => GetInt64(true);
            }
        }

        public sealed record Float : IParameterOpenApiType<float>
        {
            public static OpenApiSchema GetSchema() => GetFloat(false);

            public sealed record Nullable : IParameterOpenApiType<float?>
            {
                public static OpenApiSchema GetSchema() => GetFloat(true);
            }
        }

        public sealed record Double : IParameterOpenApiType<double>
        {
            public static OpenApiSchema GetSchema() => GetDouble(false);

            public sealed record Nullable : IParameterOpenApiType<double?>
            {
                public static OpenApiSchema GetSchema() => GetDouble(true);
            }
        }

        public sealed record Decimal : IParameterOpenApiType<decimal>
        {
            public static OpenApiSchema GetSchema() => GetDouble(false);

            public sealed record Nullable : IParameterOpenApiType<decimal?>
            {
                public static OpenApiSchema GetSchema() => GetDouble(true);
            }
        }

        public sealed record DateTime : IParameterOpenApiType<System.DateTime>
        {
            public static OpenApiSchema GetSchema() => GetDateTime(false);

            public sealed record Nullable : IParameterOpenApiType<System.DateTime?>
            {
                public static OpenApiSchema GetSchema() => GetDateTime(true);
            }
        }

        public sealed record DateTimeOffset : IParameterOpenApiType<System.DateTimeOffset>
        {
            public static OpenApiSchema GetSchema() => GetDateTime(false);

            public sealed record Nullable : IParameterOpenApiType<System.DateTimeOffset?>
            {
                public static OpenApiSchema GetSchema() => GetDateTime(true);
            }
        }

        public sealed record TimeSpan : IParameterOpenApiType<System.TimeSpan>
        {
            public static OpenApiSchema GetSchema() => GetDateSpan(false);

            public sealed record Nullable : IParameterOpenApiType<System.TimeSpan?>
            {
                public static OpenApiSchema GetSchema() => GetDateSpan(true);
            }
        }

        public sealed record Guid : IParameterOpenApiType<System.Guid>
        {
            public static OpenApiSchema GetSchema() => GetGuid(false);

            public sealed record Nullable : IParameterOpenApiType<System.Guid?>
            {
                public static OpenApiSchema GetSchema() => GetGuid(true);
            }
        }

        public sealed record Uri : IParameterOpenApiType<System.Uri>
        {
            public static OpenApiSchema GetSchema() => GetUri(false);

            public sealed record Nullable : IParameterOpenApiType<System.Uri>
            {
                public static OpenApiSchema GetSchema() => GetUri(true);
            }
        }

        public sealed record String : IParameterOpenApiType<string>
        {
            public static OpenApiSchema GetSchema() => GetString(false);

            public sealed record Nullable : IParameterOpenApiType<string?>
            {
                public static OpenApiSchema GetSchema() => GetString(true);
            }
        }

        public sealed record Array<T> : IParameterOpenApiType<T>
            where T : IEnumerable
        {
            public static OpenApiSchema GetSchema() => GetArray(false);

            public sealed record Nullable : IParameterOpenApiType<T?>
            {
                public static OpenApiSchema GetSchema() => GetArray(true);
            }
        }

        public sealed record ObjectCollection<T> : IOpenApiType<T>
            where T : class
        {
            public static OpenApiSchema GetSchema() => GetObject(false);

            public sealed record Nullable : IParameterOpenApiType<T?>
            {
                public static OpenApiSchema GetSchema() => GetObject(true);
            }
        }

        public sealed record FormFileCollection<T> : IOpenApiType<T>
            where T : IFormFileCollection
        {
            public static OpenApiSchema GetSchema() => GetObject(false);

            public sealed record Nullable : IParameterOpenApiType<T?>
            {
                public static OpenApiSchema GetSchema() => GetObject(true);
            }
        }

        public sealed record FormFile<T> : IOpenApiType<T>
            where T : IFormFile
        {
            public static OpenApiSchema GetSchema() => GetObject(false);

            public sealed record Nullable : IParameterOpenApiType<T?>
            {
                public static OpenApiSchema GetSchema() => GetObject(true);
            }
        }

        public sealed record Dictionary<T> : IOpenApiType<T>
            where T : IDictionary
        {
            public static OpenApiSchema GetSchema() => GetObject(false);

            public sealed record Nullable : IParameterOpenApiType<T?>
            {
                public static OpenApiSchema GetSchema() => GetObject(true);
            }
        }
    }
}