namespace FeatureSlice.Html;

public sealed record src(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("src", Value);
}

public static partial class HTML
{
    public static src src(string value) => new (value);
}
