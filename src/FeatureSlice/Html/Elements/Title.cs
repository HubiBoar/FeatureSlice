namespace FeatureSlice.Html;

public sealed record Title(string Value) : IElement
{
    public string ToHtml() => HtmlHelper.Value("title", Value);
}

public static partial class HTML
{
    public static Title Title(string title)
    {
        return new Title(title);
    }
}
