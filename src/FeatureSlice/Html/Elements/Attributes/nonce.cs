namespace FeatureSlice.Html;

public sealed record nonce(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("nonce", Value);
}

public static partial class HTML
{
    public static nonce nonce(string value) => new (value);
}
