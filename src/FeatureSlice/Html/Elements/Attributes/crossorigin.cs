namespace FeatureSlice.Html;

public sealed record crossorigin(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("crossorigin", Value);
}

public static partial class HTML
{
    public static crossorigin crossorigin(string value) => new (value);
}
