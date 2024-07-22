using System.Collections;

namespace FeatureSlice;

public static partial class From
{
    public static partial class Header
    {
        public static FromHeaderBinder<T, TParam> Base<T, TParam>(string name, bool required = true)
            where TParam : IParameterOpenApiType<T>
        {
            return new (name, required);
        }

        public static FromHeaderBinder<bool, OpenApi.Bool> Bool(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<byte, OpenApi.Byte> Byte(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<int, OpenApi.Int> Int(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<uint, OpenApi.UInt> UInt(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<ushort, OpenApi.UShort> UShort(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<long, OpenApi.Long> Long(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<ulong, OpenApi.ULong> ULong(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<float, OpenApi.Float> Float(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<double, OpenApi.Double> Double(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<decimal, OpenApi.Decimal> Decimal(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<DateTime, OpenApi.DateTime> DateTime(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<DateTimeOffset, OpenApi.DateTimeOffset> DateTimeOffset(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<TimeSpan, OpenApi.TimeSpan> TimeSpan(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<Guid, OpenApi.Guid> Guid(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<Uri, OpenApi.Uri> Uri(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<string, OpenApi.String> String(string name, bool required = true) => new(name, required);
        public static FromHeaderBinder<T, OpenApi.Array<T>> Array<T>(string name, bool required = true) where T : IEnumerable => new(name, required);

        public static partial class Nullable
        {
            public static FromHeaderBinder<bool?, OpenApi.Bool.Nullable> Bool(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<byte?, OpenApi.Byte.Nullable> Byte(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<int?, OpenApi.Int.Nullable> Int(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<uint?, OpenApi.UInt.Nullable> UInt(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<ushort?, OpenApi.UShort.Nullable> UShort(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<long?, OpenApi.Long.Nullable> Long(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<ulong?, OpenApi.ULong.Nullable> ULong(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<float?, OpenApi.Float.Nullable> Float(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<double?, OpenApi.Double.Nullable> Double(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<decimal?, OpenApi.Decimal.Nullable> Decimal(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<DateTime?, OpenApi.DateTime.Nullable> DateTime(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<DateTimeOffset?, OpenApi.DateTimeOffset.Nullable> DateTimeOffset(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<TimeSpan?, OpenApi.TimeSpan.Nullable> TimeSpan(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<Guid?, OpenApi.Guid.Nullable> Guid(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<Uri?, OpenApi.Uri.Nullable> Uri(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<string?, OpenApi.String.Nullable> String(string name, bool required = false) => new(name, required);
            public static FromHeaderBinder<T?, OpenApi.Array<T>.Nullable> Array<T>(string name, bool required = false) where T : IEnumerable => new(name, required);
        }
    }
}