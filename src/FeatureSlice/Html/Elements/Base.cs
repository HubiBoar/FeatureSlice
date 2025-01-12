namespace FeatureSlice.Html;

public sealed record Base(Attribute.Href Href, Attribute.Target? Target) : IElement
{
    public string ToHtml() => HtmlHelper.Simple("base", [ Href, Target ]);
}

public static partial class HTML
{
    public static Base Base(Attribute.Href href, Attribute.Target? target = null)
    {
        return new Base(href, target);
    }
}
