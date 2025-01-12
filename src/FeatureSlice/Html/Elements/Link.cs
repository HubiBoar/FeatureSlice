namespace FeatureSlice.Html;

public sealed record LinkList(IReadOnlyCollection<Link> Collection) : IElement
{
    public string ToHtml() => HtmlHelper.Collection(Collection);
}

public sealed record Link
(
    href href,
    rel rel,
    media? media,
    type.link? type,
    sizes? sizes,
    hrefset? hrefset,
    integrity? integrity,
    crossorigin? crossorigin,
    as_? as_
)
: IElement
{
    public string ToHtml() => HtmlHelper.Simple
    (
        "link",
        [ href, rel, media, type, sizes, hrefset, integrity, crossorigin, as_ ]
    ); 

    public static implicit operator LinkList(Link link) => new ([ link ]);
}

public static partial class HTML
{
    public static Link Link
    (
        href href,
        rel rel,
        media? media = null,
        type.link? type = null,
        sizes? sizes = null,
        hrefset? hrefSet = null,
        integrity? integrity = null,
        crossorigin? crossorigin = null,
        as_? as_ = null
    )
    {
        return new Link(href, rel, media, type, sizes, hrefSet, integrity, crossorigin, as_);
    }

    public static LinkList Link
    (
        params FeatureSlice.Html.Link[] links
    )
    {
        return new LinkList(links);  
    }
}
