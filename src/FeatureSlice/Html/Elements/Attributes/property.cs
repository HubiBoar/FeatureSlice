namespace FeatureSlice.Html;

public sealed record property(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("property", Value);
}

public static partial class HTML
{
    public static property property(string value) => new (value);
}
