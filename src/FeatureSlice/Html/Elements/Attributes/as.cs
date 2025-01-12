namespace FeatureSlice.Html;

public sealed record as_(string Value) : IAttribute
{
    public string ToHtml() => HtmlHelper.Attribute("as", Value);
}

public static partial class HTML
{
    public static as_ as_(string value) => new (value);
}
