namespace FeatureSlice.Html;

public sealed record Style
(
    type.style? Type,
    media? Media,
    nonce? Nonce,
    string Value
)
: IElement
{
    public string ToHtml() => HtmlHelper.Value("style", [ Type, Media, Nonce ], Value); 

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
        type.style? type = null,
        media? media = null,
        nonce? nonce = null
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
