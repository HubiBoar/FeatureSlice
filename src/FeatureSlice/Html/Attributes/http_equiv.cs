namespace FeatureSlice.Html;

public sealed record http_equiv(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("http-equiv", Value);
}

public static partial class HTML
{
    public static http_equiv http_equiv(string value) => new (value);
}
