namespace FeatureSlice.Html;

public sealed record content(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("content", Value);
}

public static partial class HTML
{
    public static content content(string value) => new (value);
}
