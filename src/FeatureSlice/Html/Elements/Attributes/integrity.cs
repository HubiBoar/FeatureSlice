namespace FeatureSlice.Html;

public sealed record integrity(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("integrity", Value);
}

public static partial class HTML
{
    public static integrity integrity(string value) => new integrity(value);
}
