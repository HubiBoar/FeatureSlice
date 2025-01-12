namespace FeatureSlice.Html;

public sealed record rel(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("rel", Value);
}

public static partial class HTML
{
    public static rel rel(string value) => new (value);
}
