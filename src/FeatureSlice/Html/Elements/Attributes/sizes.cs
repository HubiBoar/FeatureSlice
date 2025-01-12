namespace FeatureSlice.Html;

public sealed record sizes(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("sizes", Value);
}

public static partial class HTML
{
    public static sizes sizes(string value) => new (value);
}
