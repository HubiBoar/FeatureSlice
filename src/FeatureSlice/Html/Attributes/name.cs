namespace FeatureSlice.Html;

public sealed record name(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("name", Value);
}

public static partial class HTML
{
    public static name name(string value) => new (value);
}
