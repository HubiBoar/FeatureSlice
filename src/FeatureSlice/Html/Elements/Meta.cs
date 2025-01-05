namespace FeatureSlice.Html;

public static class Meta
{
    public static FeatureSlice.Html.IMeta.Charset charset(Attribute.Charset charset) => new (charset); 
    public static FeatureSlice.Html.IMeta.HTTPEquiv http_equiv(string http_equiv, string content) => new (http_equiv, content); 
    public static FeatureSlice.Html.IMeta.Nme name(string name, string content) => new (name, content); 
    public static FeatureSlice.Html.IMeta.Property property(string property, string content) => new (property, content); 
}

public interface IMeta : IElement
{
    public interface IMany : IMeta;

    static string IElement.Name { get; } = "meta";

    public sealed record Charset(Attribute.Charset Value) : IMeta;
    public sealed record HTTPEquiv(string Header, string Content) : IMany;
    public sealed record Nme(string Header, string Content) : IMany;
    public sealed record Property(string Header, string Content) : IMany;
}


public sealed record MetaList(IReadOnlyList<IMeta> List);

public static partial class HTML
{
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
