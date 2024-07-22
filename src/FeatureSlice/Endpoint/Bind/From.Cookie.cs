using System.Collections;

namespace FeatureSlice;

public static partial class From
{
    public static partial class Cookie
    {
        public static FromCookieBinder<T, TParam> Base<T, TParam>(string name, bool required = true)
            where TParam : IParameterOpenApiType<T>
        {
            return new (name, required);
        }

        public static FromCookieBinder<bool, OpenApi.Bool> Bool(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<byte, OpenApi.Byte> Byte(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<int, OpenApi.Int> Int(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<uint, OpenApi.UInt> UInt(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<ushort, OpenApi.UShort> UShort(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<long, OpenApi.Long> Long(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<ulong, OpenApi.ULong> ULong(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<float, OpenApi.Float> Float(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<double, OpenApi.Double> Double(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<decimal, OpenApi.Decimal> Decimal(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<DateTime, OpenApi.DateTime> DateTime(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<DateTimeOffset, OpenApi.DateTimeOffset> DateTimeOffset(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<TimeSpan, OpenApi.TimeSpan> TimeSpan(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<Guid, OpenApi.Guid> Guid(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<Uri, OpenApi.Uri> Uri(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<string, OpenApi.String> String(string name, bool required = true) => new(name, required);
        public static FromCookieBinder<T, OpenApi.Array<T>> Array<T>(string name, bool required = true) where T : IEnumerable => new(name, required);

        public static partial class Nullable
        {
            public static FromCookieBinder<bool?, OpenApi.Bool.Nullable> Bool(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<byte?, OpenApi.Byte.Nullable> Byte(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<int?, OpenApi.Int.Nullable> Int(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<uint?, OpenApi.UInt.Nullable> UInt(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<ushort?, OpenApi.UShort.Nullable> UShort(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<long?, OpenApi.Long.Nullable> Long(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<ulong?, OpenApi.ULong.Nullable> ULong(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<float?, OpenApi.Float.Nullable> Float(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<double?, OpenApi.Double.Nullable> Double(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<decimal?, OpenApi.Decimal.Nullable> Decimal(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<DateTime?, OpenApi.DateTime.Nullable> DateTime(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<DateTimeOffset?, OpenApi.DateTimeOffset.Nullable> DateTimeOffset(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<TimeSpan?, OpenApi.TimeSpan.Nullable> TimeSpan(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<Guid?, OpenApi.Guid.Nullable> Guid(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<Uri?, OpenApi.Uri.Nullable> Uri(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<string?, OpenApi.String.Nullable> String(string name, bool required = false) => new(name, required);
            public static FromCookieBinder<T?, OpenApi.Array<T>.Nullable> Array<T>(string name, bool required = false) where T : IEnumerable => new(name, required);
        }
    }
}