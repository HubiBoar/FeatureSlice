namespace FeatureSlice.Html;

public interface IMeta : IElement
{
    public interface IMany : IMeta;

    public sealed record Charset(charset charset) : IMeta
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ charset ]);

        public static implicit operator MetaList(Charset value) => new ([ value ]);
    }

    public sealed record HttpEquiv(http_equiv header, content content) : IMany
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ header, content ]);

        public static implicit operator MetaList(HttpEquiv value) => new ([ value ]);
    }

    public sealed record Name(name header, content content) : IMany
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ header, content ]);

        public static implicit operator MetaList(Name value) => new ([ value ]);
    }

    public sealed record Property(property header, content content) : IMany
    {
        public string ToHtml() => HtmlHelper.Simple("meta", [ header, content ]);

        public static implicit operator MetaList(Property value) => new ([ value ]);
    }
}

public sealed record MetaList(IReadOnlyCollection<IMeta> Collection) : IElement
{
    public string ToHtml() => HtmlHelper.Collection(Collection);
}

public static partial class HTML
{
    public static FeatureSlice.Html.IMeta.Charset Meta(charset charset) => new (charset); 
    public static FeatureSlice.Html.IMeta.HttpEquiv Meta(http_equiv http_equiv, content content) => new (http_equiv, content); 
    public static FeatureSlice.Html.IMeta.Name Meta(name name, content content) => new (name, content); 
    public static FeatureSlice.Html.IMeta.Property Meta(property name, content content) => new (name, content); 

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
