using System.Collections;

namespace FeatureSlice;

public static partial class From
{
    public static partial class Query
    {
        public static FromQueryBinder<T, TParam> Base<T, TParam>(string name, bool required = true)
            where TParam : IParameterOpenApiType<T>
        {
            return new (name, required);
        }

        public static FromQueryBinder<bool, OpenApi.Bool> Bool(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<byte, OpenApi.Byte> Byte(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<int, OpenApi.Int> Int(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<uint, OpenApi.UInt> UInt(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<ushort, OpenApi.UShort> UShort(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<long, OpenApi.Long> Long(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<ulong, OpenApi.ULong> ULong(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<float, OpenApi.Float> Float(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<double, OpenApi.Double> Double(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<decimal, OpenApi.Decimal> Decimal(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<DateTime, OpenApi.DateTime> DateTime(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<DateTimeOffset, OpenApi.DateTimeOffset> DateTimeOffset(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<TimeSpan, OpenApi.TimeSpan> TimeSpan(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<Guid, OpenApi.Guid> Guid(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<Uri, OpenApi.Uri> Uri(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<string, OpenApi.String> String(string name, bool required = true) => new(name, required);
        public static FromQueryBinder<T, OpenApi.Array<T>> Array<T>(string name, bool required = true) where T : IEnumerable => new(name, required);

        public static partial class Nullable
        {
            public static FromQueryBinder<bool?, OpenApi.Bool.Nullable> Bool(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<byte?, OpenApi.Byte.Nullable> Byte(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<int?, OpenApi.Int.Nullable> Int(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<uint?, OpenApi.UInt.Nullable> UInt(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<ushort?, OpenApi.UShort.Nullable> UShort(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<long?, OpenApi.Long.Nullable> Long(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<ulong?, OpenApi.ULong.Nullable> ULong(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<float?, OpenApi.Float.Nullable> Float(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<double?, OpenApi.Double.Nullable> Double(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<decimal?, OpenApi.Decimal.Nullable> Decimal(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<DateTime?, OpenApi.DateTime.Nullable> DateTime(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<DateTimeOffset?, OpenApi.DateTimeOffset.Nullable> DateTimeOffset(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<TimeSpan?, OpenApi.TimeSpan.Nullable> TimeSpan(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<Guid?, OpenApi.Guid.Nullable> Guid(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<Uri?, OpenApi.Uri.Nullable> Uri(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<string?, OpenApi.String.Nullable> String(string name, bool required = false) => new(name, required);
            public static FromQueryBinder<T?, OpenApi.Array<T>.Nullable> Array<T>(string name, bool required = false) where T : IEnumerable => new(name, required);
        }
    }
}