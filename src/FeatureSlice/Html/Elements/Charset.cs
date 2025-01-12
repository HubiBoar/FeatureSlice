namespace FeatureSlice.Html;

public static partial class Attribute
{
    public sealed record Charset(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("charset", Value);

        public enum Type
        {
            UTF_8,
            UTF_16,
            UTF_32,
            ISO_8859_1,
            ISO_8859_2,
            ISO_8859_3,
            ISO_8859_4,
            ISO_8859_5,
            ISO_8859_6,
            ISO_8859_7,
            ISO_8859_8,
            ISO_8859_9,
            ISO_8859_10,
            ISO_8859_11,
            ISO_8859_13,
            ISO_8859_14,
            ISO_8859_15,
            ISO_8859_16,
            KOI8_R,
            KOI8_U,
            MacRoman,
            Windows_1250,
            Windows_1251,
            Windows_1252,
            Windows_1253,
            Windows_1254,
            Windows_1255,
            Windows_1256,
            Windows_1257,
            Windows_1258,
            Big5,
            GB2312,
            GBK,
            Shift_JIS,
            EUC_JP,
            TIS_620,
            VISCII,
            ISO_2022_JP,
            EUC_KR,
            GB18030,
            EBCDIC,
            Shift_JISX0213,
            X_Mac_Cyrillic,
        }

        public static Charset Create(Type charset)
        {
            var value = charset switch
            {
                Type.UTF_8 => "UTF-8",
                Type.UTF_16 => "UTF-16",
                Type.UTF_32 => "UTF-32",
                Type.ISO_8859_1 => "ISO-8859-1",
                Type.ISO_8859_2 => "ISO-8859-2",
                Type.ISO_8859_3 => "ISO-8859-3",
                Type.ISO_8859_4 => "ISO-8859-4",
                Type.ISO_8859_5 => "ISO-8859-5",
                Type.ISO_8859_6 => "ISO-8859-6",
                Type.ISO_8859_7 => "ISO-8859-7",
                Type.ISO_8859_8 => "ISO-8859-8",
                Type.ISO_8859_9 => "ISO-8859-9",
                Type.ISO_8859_10 => "ISO-8859-10",
                Type.ISO_8859_11 => "ISO-8859-11",
                Type.ISO_8859_13 => "ISO-8859-13",
                Type.ISO_8859_14 => "ISO-8859-14",
                Type.ISO_8859_15 => "ISO-8859-15",
                Type.ISO_8859_16 => "ISO-8859-16",
                Type.KOI8_R => "KOI8-R",
                Type.KOI8_U => "KOI8-U",
                Type.MacRoman => "MacRoman",
                Type.Windows_1250 => "Windows-1250",
                Type.Windows_1251 => "Windows-1251",
                Type.Windows_1252 => "Windows-1252",
                Type.Windows_1253 => "Windows-1253",
                Type.Windows_1254 => "Windows-1254",
                Type.Windows_1255 => "Windows-1255",
                Type.Windows_1256 => "Windows-1256",
                Type.Windows_1257 => "Windows-1257",
                Type.Windows_1258 => "Windows-1258",
                Type.Big5 => "Big5",
                Type.GB2312 => "GB2312",
                Type.GBK => "GBK",
                Type.Shift_JIS => "Shift_JIS",
                Type.EUC_JP => "EUC-JP",
                Type.TIS_620 => "TIS-620",
                Type.VISCII => "VISCII",
                Type.ISO_2022_JP => "ISO-2022-JP",
                Type.EUC_KR => "EUC-KR",
                Type.GB18030 => "GB18030",
                Type.EBCDIC => "EBCDIC",
                Type.Shift_JISX0213 => "Shift_JISX0213",
                Type.X_Mac_Cyrillic => "X-Mac-Cyrillic",
                _ => throw new ArgumentOutOfRangeException(nameof(charset), charset, null)
            };

            return new Charset(value);
        }
    }
}

public static partial class HTML
{
    public static class charset
    {
        public static Attribute.Charset Create(string value) => new Attribute.Charset(value); 

        public static Attribute.Charset UTF_8             { get; } = Attribute.Charset.Create(Attribute.Charset.Type.UTF_8);
        public static Attribute.Charset UTF_16            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.UTF_16);
        public static Attribute.Charset UTF_32            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.UTF_32);
        public static Attribute.Charset ISO_8859_1        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_1);
        public static Attribute.Charset ISO_8859_2        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_2);
        public static Attribute.Charset ISO_8859_3        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_3);
        public static Attribute.Charset ISO_8859_4        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_4);
        public static Attribute.Charset ISO_8859_5        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_5);
        public static Attribute.Charset ISO_8859_6        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_6);
        public static Attribute.Charset ISO_8859_7        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_7);
        public static Attribute.Charset ISO_8859_8        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_8);
        public static Attribute.Charset ISO_8859_9        { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_9);
        public static Attribute.Charset ISO_8859_10       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_10);
        public static Attribute.Charset ISO_8859_11       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_11);
        public static Attribute.Charset ISO_8859_13       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_13);
        public static Attribute.Charset ISO_8859_14       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_14);
        public static Attribute.Charset ISO_8859_15       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_15);
        public static Attribute.Charset ISO_8859_16       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_8859_16);
        public static Attribute.Charset KOI8_R            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.KOI8_R);
        public static Attribute.Charset KOI8_U            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.KOI8_U);
        public static Attribute.Charset MacRoman          { get; } = Attribute.Charset.Create(Attribute.Charset.Type.MacRoman);
        public static Attribute.Charset Windows_1250      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1250);
        public static Attribute.Charset Windows_1251      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1251);
        public static Attribute.Charset Windows_1252      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1252);
        public static Attribute.Charset Windows_1253      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1253);
        public static Attribute.Charset Windows_1254      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1254);
        public static Attribute.Charset Windows_1255      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1255);
        public static Attribute.Charset Windows_1256      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1256);
        public static Attribute.Charset Windows_1257      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1257);
        public static Attribute.Charset Windows_1258      { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Windows_1258);
        public static Attribute.Charset Big5              { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Big5);
        public static Attribute.Charset GB2312            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.GB2312);
        public static Attribute.Charset GBK               { get; } = Attribute.Charset.Create(Attribute.Charset.Type.GBK);
        public static Attribute.Charset Shift_JIS         { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Shift_JIS);
        public static Attribute.Charset EUC_JP            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.EUC_JP);
        public static Attribute.Charset TIS_620           { get; } = Attribute.Charset.Create(Attribute.Charset.Type.TIS_620);
        public static Attribute.Charset VISCII            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.VISCII);
        public static Attribute.Charset ISO_2022_JP       { get; } = Attribute.Charset.Create(Attribute.Charset.Type.ISO_2022_JP);
        public static Attribute.Charset EUC_KR            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.EUC_KR);
        public static Attribute.Charset GB18030           { get; } = Attribute.Charset.Create(Attribute.Charset.Type.GB18030);
        public static Attribute.Charset EBCDIC            { get; } = Attribute.Charset.Create(Attribute.Charset.Type.EBCDIC);
        public static Attribute.Charset Shift_JISX0213    { get; } = Attribute.Charset.Create(Attribute.Charset.Type.Shift_JISX0213);
        public static Attribute.Charset X_Mac_Cyrillic    { get; } = Attribute.Charset.Create(Attribute.Charset.Type.X_Mac_Cyrillic);
    }
}
