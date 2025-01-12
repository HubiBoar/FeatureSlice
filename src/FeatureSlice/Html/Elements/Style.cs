namespace FeatureSlice.Html;

public static partial class Attribute
{
    public static class Style
    {
        public sealed record Type(string Value) : IAttribute
        {
            public string ToHtml() => HtmlHelper.Attribute("type", Value);
        }
    }

    public sealed record Nonce(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("nonce", Value);
    }
}

public sealed record Style
(
    Attribute.Style.Type? Type,
    Attribute.Media? Media,
    Attribute.Nonce? Nonce,
    string Value
)
: IElement
{
    public string ToHtml() => HtmlHelper.Value("style", [ Type, Media, Nonce ], Value); 

    public static class type
    {
        public static Attribute.Style.Type css { get; } = new ("text/css");
        public static Attribute.Style.Type x_scss { get; } = new ("text/x-scss");
        public static Attribute.Style.Type x_less { get; } = new ("text/x-less");
        public static Attribute.Style.Type raw(string value) => new (value);
    }

    public static implicit operator StyleList(Style style) => new ([ style ]);
}

public sealed record StyleList(IReadOnlyCollection<Style> Collection) : IElement
{
    public string ToHtml() => HtmlHelper.Collection(Collection);
}

public static partial class HTML
{
    public static Func<string, Style> Style
    (
        Attribute.Style.Type? type = null,
        Attribute.Media? media = null,
        Attribute.Nonce? nonce = null
    )
    {
        return (value) => new Style(type, media, nonce, value);
    }

    public static StyleList Style
    (
        params Style[] list
    )
    {
        return new StyleList(list);  
    }
}
