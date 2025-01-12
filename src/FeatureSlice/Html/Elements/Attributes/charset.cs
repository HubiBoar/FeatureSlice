namespace FeatureSlice.Html;

public sealed record charset(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("charset", Value);

    public static charset UTF_8             { get; } = charset.Create(Type.UTF_8);
    public static charset UTF_16            { get; } = charset.Create(Type.UTF_16);
    public static charset UTF_32            { get; } = charset.Create(Type.UTF_32);
    public static charset ISO_8859_1        { get; } = charset.Create(Type.ISO_8859_1);
    public static charset ISO_8859_2        { get; } = charset.Create(Type.ISO_8859_2);
    public static charset ISO_8859_3        { get; } = charset.Create(Type.ISO_8859_3);
    public static charset ISO_8859_4        { get; } = charset.Create(Type.ISO_8859_4);
    public static charset ISO_8859_5        { get; } = charset.Create(Type.ISO_8859_5);
    public static charset ISO_8859_6        { get; } = charset.Create(Type.ISO_8859_6);
    public static charset ISO_8859_7        { get; } = charset.Create(Type.ISO_8859_7);
    public static charset ISO_8859_8        { get; } = charset.Create(Type.ISO_8859_8);
    public static charset ISO_8859_9        { get; } = charset.Create(Type.ISO_8859_9);
    public static charset ISO_8859_10       { get; } = charset.Create(Type.ISO_8859_10);
    public static charset ISO_8859_11       { get; } = charset.Create(Type.ISO_8859_11);
    public static charset ISO_8859_13       { get; } = charset.Create(Type.ISO_8859_13);
    public static charset ISO_8859_14       { get; } = charset.Create(Type.ISO_8859_14);
    public static charset ISO_8859_15       { get; } = charset.Create(Type.ISO_8859_15);
    public static charset ISO_8859_16       { get; } = charset.Create(Type.ISO_8859_16);
    public static charset KOI8_R            { get; } = charset.Create(Type.KOI8_R);
    public static charset KOI8_U            { get; } = charset.Create(Type.KOI8_U);
    public static charset MacRoman          { get; } = charset.Create(Type.MacRoman);
    public static charset Windows_1250      { get; } = charset.Create(Type.Windows_1250);
    public static charset Windows_1251      { get; } = charset.Create(Type.Windows_1251);
    public static charset Windows_1252      { get; } = charset.Create(Type.Windows_1252);
    public static charset Windows_1253      { get; } = charset.Create(Type.Windows_1253);
    public static charset Windows_1254      { get; } = charset.Create(Type.Windows_1254);
    public static charset Windows_1255      { get; } = charset.Create(Type.Windows_1255);
    public static charset Windows_1256      { get; } = charset.Create(Type.Windows_1256);
    public static charset Windows_1257      { get; } = charset.Create(Type.Windows_1257);
    public static charset Windows_1258      { get; } = charset.Create(Type.Windows_1258);
    public static charset Big5              { get; } = charset.Create(Type.Big5);
    public static charset GB2312            { get; } = charset.Create(Type.GB2312);
    public static charset GBK               { get; } = charset.Create(Type.GBK);
    public static charset Shift_JIS         { get; } = charset.Create(Type.Shift_JIS);
    public static charset EUC_JP            { get; } = charset.Create(Type.EUC_JP);
    public static charset TIS_620           { get; } = charset.Create(Type.TIS_620);
    public static charset VISCII            { get; } = charset.Create(Type.VISCII);
    public static charset ISO_2022_JP       { get; } = charset.Create(Type.ISO_2022_JP);
    public static charset EUC_KR            { get; } = charset.Create(Type.EUC_KR);
    public static charset GB18030           { get; } = charset.Create(Type.GB18030);
    public static charset EBCDIC            { get; } = charset.Create(Type.EBCDIC);
    public static charset Shift_JISX0213    { get; } = charset.Create(Type.Shift_JISX0213);
    public static charset X_Mac_Cyrillic    { get; } = charset.Create(Type.X_Mac_Cyrillic);

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

    public static charset Create(Type charset)
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

        return new charset(value);
    }
}

public static partial class HTML
{
    public static charset charset(string value) => new (value);
}
