namespace FeatureSlice.Html;

public abstract record Meta : IElement
{
    public static string Name { get; } = "meta";

    public sealed record Charset(Attribute.Charset Value) : Meta;
    public sealed record HTTPEquiv(string Header, string Content) : Meta;
    public sealed record Nme(string Header, string Content) : Meta;
    public sealed record Property(string Header, string Content) : Meta;
}

public sealed record MetaList
{
    public IReadOnlyList<Meta> Meta => _meta;
    private readonly List<Meta> _meta = new();

    public static FeatureSlice.Html.Meta.Charset charset(Attribute.Charset charset) => new (charset); 
    public static FeatureSlice.Html.Meta.HTTPEquiv http_equiv(string http_equiv, string content) => new (http_equiv, content); 
    public static FeatureSlice.Html.Meta.Nme name(string name, string content) => new (name, content); 
    public static FeatureSlice.Html.Meta.Property property(string property, string content) => new (property, content); 
}

public static partial class HTML
{
    public static class Meta
    {
    }
}
