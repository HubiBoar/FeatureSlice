namespace FeatureSlice.Html;

public static partial class Attribute
{
    public sealed record Href(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("href", Value);
    }
}

public static partial class HTML
{
    public static Attribute.Href href(string value)
    {
        return new Attribute.Href(value);
    }
}
