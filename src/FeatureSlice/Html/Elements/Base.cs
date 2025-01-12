namespace FeatureSlice.Html;

public sealed record Base(href href, target? target) : IElement
{
    public string ToHtml() => HtmlHelper.Simple("base", [ href, target ]);
}

public static partial class HTML
{
    public static Base Base(href href, target? target = null)
    {
        return new Base(href, target);
    }
}
