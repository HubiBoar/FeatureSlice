namespace FeatureSlice.Html;

public sealed record media(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("media", Value);
}

public static partial class HTML
{
    public static media media(string value) => new (value);
}
