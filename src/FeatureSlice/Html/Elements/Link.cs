namespace FeatureSlice.Html;

public static partial class Attribute
{
    public sealed record Rel(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("rel", Value);
    }

    public sealed record Type(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("type", Value);
    }

    public sealed record Media(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("media", Value);
    }

    public sealed record Sizes(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("sizes", Value);
    }

    public sealed record HrefSet(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("hrefset", Value);
    }

    public sealed record Integrity(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("integrity", Value);
    }

    public sealed record CrossOrigin(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("crossorigin", Value);
    }

    public sealed record As(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("as", Value);
    }
}

public sealed record LinkList(IReadOnlyCollection<Link> Collection) : IElement
{
    public string ToHtml() => HtmlHelper.Collection(Collection);
}

public sealed record Link
(
    Attribute.Href Href,
    Attribute.Rel Rel,
    Attribute.Media? Media,
    Attribute.Type? Type,
    Attribute.Sizes? Sizes,
    Attribute.HrefSet? HrefSet,
    Attribute.Integrity? Integrity,
    Attribute.CrossOrigin? CrossOrigin,
    Attribute.As? As
)
: IElement
{
    public string ToHtml() => HtmlHelper.Simple
    (
        "link",
        [ Href, Rel, Media, Type, Sizes, HrefSet, Integrity, CrossOrigin, As ]
    ); 

    public static implicit operator LinkList(Link link) => new ([ link ]);
}

public static partial class HTML
{
    public static Link Link
    (
        Attribute.Href href,
        Attribute.Rel rel,
        Attribute.Media? media = null,
        Attribute.Type? type = null,
        Attribute.Sizes? sizes = null,
        Attribute.HrefSet? hrefSet = null,
        Attribute.Integrity? integrity = null,
        Attribute.CrossOrigin? crossOrigin = null,
        Attribute.As? @as = null
    )
    {
        return new Link(href, rel, media, type, sizes, hrefSet, integrity, crossOrigin, @as);
    }

    public static Attribute.Rel rel(string value) => new Attribute.Rel(value);

    public static Attribute.Type type(string value) => new Attribute.Type(value);

    public static Attribute.Media media(string value) => new Attribute.Media(value);

    public static Attribute.Sizes sizes(string value) => new Attribute.Sizes(value);

    public static Attribute.HrefSet hrefset(string value) => new Attribute.HrefSet(value);

    public static Attribute.Integrity integrity(string value) => new Attribute.Integrity(value);

    public static Attribute.CrossOrigin crossOrigin(string value) => new Attribute.CrossOrigin(value);

    public static Attribute.As @as(string value) => new Attribute.As(value);

    public static LinkList Link
    (
        params FeatureSlice.Html.Link[] links
    )
    {
        return new LinkList(links);  
    }
}
