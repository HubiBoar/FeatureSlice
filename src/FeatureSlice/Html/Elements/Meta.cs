namespace FeatureSlice.Html;

public static partial class Attribute
{
    public sealed record Name(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("name", Value);
    }

    public sealed record Property(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("property", Value);
    }

    public sealed record HTTPEquiv(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("http-equiv", Value);
    }

    public sealed record Content(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("content", Value);
    }
}

public interface IMeta : IElement
{
    public interface IMany : IMeta;

    public sealed record Charset(Attribute.Charset Value) : IMeta
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ Value ]);

        public static implicit operator MetaList(Charset value) => new ([ value ]);
    }

    public sealed record HTTPEquiv(Attribute.HTTPEquiv Header, Attribute.Content Content) : IMany
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ Header, Content ]);

        public static implicit operator MetaList(HTTPEquiv value) => new ([ value ]);
    }

    public sealed record Name(Attribute.Name Header, Attribute.Content Content) : IMany
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ Header, Content ]);

        public static implicit operator MetaList(Name value) => new ([ value ]);
    }

    public sealed record Property(Attribute.Property Header, Attribute.Content Content) : IMany
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ Header, Content ]);

        public static implicit operator MetaList(Property value) => new ([ value ]);
    }
}

public sealed record MetaList(IReadOnlyCollection<IMeta> Collection) : IElement
{
    public string ToHtml() => HtmlHelper.Collection(Collection);
}

public static partial class HTML
{
    public static FeatureSlice.Html.IMeta.Charset Meta(Attribute.Charset charset) => new (charset); 
    public static FeatureSlice.Html.IMeta.HTTPEquiv Meta(Attribute.HTTPEquiv http_equiv, Attribute.Content content) => new (http_equiv, content); 
    public static FeatureSlice.Html.IMeta.Name Meta(Attribute.Name name, Attribute.Content content) => new (name, content); 
    public static FeatureSlice.Html.IMeta.Property Meta(Attribute.Property name, Attribute.Content content) => new (name, content); 

    public static Attribute.HTTPEquiv http_equiv(string http_equiv) => new (http_equiv);
    public static Attribute.Name name(string name) => new (name);
    public static Attribute.Property property(string property) => new (property);
    public static Attribute.Content content(string content) => new (content);

    public static MetaList Meta
    (
        FeatureSlice.Html.IMeta.Charset? charset = null,
        params FeatureSlice.Html.IMeta.IMany[] metas
    )
    {
        var list = new List<FeatureSlice.Html.IMeta>();

        if (charset is not null)
        {
            list.Add(charset);
        }

        list.AddRange(metas);

        return new MetaList(list);  
    }
}
