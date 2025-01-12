namespace FeatureSlice.Html;

public sealed record href(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("href", Value);
}

public static partial class HTML
{
    public static href href(string value) => new (value);
}
