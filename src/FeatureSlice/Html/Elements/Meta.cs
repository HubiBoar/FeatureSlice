namespace FeatureSlice.Html;

public static partial class Attribute
{
    public sealed record Nme(string Value) : IAttribute
    {
        public static string Name { get; } = "name";
    }

    public sealed record Property(string Value) : IAttribute
    {
        public static string Name { get; } = "property";
    }

    public sealed record HTTPEquiv(string Value) : IAttribute
    {
        public static string Name { get; } = "http-equiv";
    }

    public sealed record Content(string Value) : IAttribute
    {
        public static string Name { get; } = "content";
    }
}

public interface IMeta : IElement
{
    public interface IMany : IMeta;

    static string IElement.Name { get; } = "meta";

    public sealed record Charset(Attribute.Charset Value) : IMeta;
    public sealed record HTTPEquiv(Attribute.HTTPEquiv Header, Attribute.Content Content) : IMany;
    public sealed record Nme(Attribute.Nme Header, Attribute.Content Content) : IMany;
    public sealed record Property(Attribute.Property Header, Attribute.Content Content) : IMany;
}

public sealed record MetaList(IReadOnlyList<IMeta> List);

public static partial class HTML
{
    public static FeatureSlice.Html.IMeta.Charset Meta(Attribute.Charset charset) => new (charset); 
    public static FeatureSlice.Html.IMeta.HTTPEquiv Meta(Attribute.HTTPEquiv http_equiv, Attribute.Content content) => new (http_equiv, content); 
    public static FeatureSlice.Html.IMeta.Nme Meta(Attribute.Nme name, Attribute.Content content) => new (name, content); 
    public static FeatureSlice.Html.IMeta.Property Meta(Attribute.Property name, Attribute.Content content) => new (name, content); 

    public static Attribute.HTTPEquiv http_equiv(string http_equiv) => new (http_equiv);
    public static Attribute.Nme name(string name) => new (name);
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
