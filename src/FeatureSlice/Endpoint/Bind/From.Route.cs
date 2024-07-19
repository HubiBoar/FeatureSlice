using System.Collections;

namespace FeatureSlice;

public static partial class From
{
    public static partial class Route
    {
        public static FromRouteBinder<T, TParam> Base<T, TParam>(string name, bool required = true)
            where TParam : IParameterOpenApiType<T>
        {
            return new (name, required);
        }

        public static FromRouteBinder<bool, OpenApi.Bool> Bool(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<byte, OpenApi.Byte> Byte(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<int, OpenApi.Int> Int(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<uint, OpenApi.UInt> UInt(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<ushort, OpenApi.UShort> UShort(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<long, OpenApi.Long> Long(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<ulong, OpenApi.ULong> ULong(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<float, OpenApi.Float> Float(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<double, OpenApi.Double> Double(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<decimal, OpenApi.Decimal> Decimal(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<DateTime, OpenApi.DateTime> DateTime(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<DateTimeOffset, OpenApi.DateTimeOffset> DateTimeOffset(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<TimeSpan, OpenApi.TimeSpan> TimeSpan(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<Guid, OpenApi.Guid> Guid(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<Uri, OpenApi.Uri> Uri(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<string, OpenApi.String> String(string name, bool required = true) => new(name, required);
        public static FromRouteBinder<T, OpenApi.Array<T>> Array<T>(string name, bool required = true) where T : IEnumerable => new(name, required);

        public static partial class Nullable
        {
            public static FromRouteBinder<bool?, OpenApi.Bool.Nullable> Bool(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<byte?, OpenApi.Byte.Nullable> Byte(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<int?, OpenApi.Int.Nullable> Int(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<uint?, OpenApi.UInt.Nullable> UInt(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<ushort?, OpenApi.UShort.Nullable> UShort(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<long?, OpenApi.Long.Nullable> Long(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<ulong?, OpenApi.ULong.Nullable> ULong(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<float?, OpenApi.Float.Nullable> Float(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<double?, OpenApi.Double.Nullable> Double(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<decimal?, OpenApi.Decimal.Nullable> Decimal(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<DateTime?, OpenApi.DateTime.Nullable> DateTime(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<DateTimeOffset?, OpenApi.DateTimeOffset.Nullable> DateTimeOffset(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<TimeSpan?, OpenApi.TimeSpan.Nullable> TimeSpan(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<Guid?, OpenApi.Guid.Nullable> Guid(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<Uri?, OpenApi.Uri.Nullable> Uri(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<string?, OpenApi.String.Nullable> String(string name, bool required = false) => new(name, required);
            public static FromRouteBinder<T?, OpenApi.Array<T>.Nullable> Array<T>(string name, bool required = false) where T : IEnumerable => new(name, required);
        }
    }
}