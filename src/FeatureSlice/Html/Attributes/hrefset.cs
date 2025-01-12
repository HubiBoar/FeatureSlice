namespace FeatureSlice.Html;

public sealed record hrefset(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("hrefset", Value);
}

public static partial class HTML
{
    public static hrefset hrefset(string value) => new (value);
}
