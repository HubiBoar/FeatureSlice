namespace FeatureSlice.Html;

public sealed record defer_() : IAttribute
{
    public string ToHtml() => "defer";
}

public static partial class HTML
{
    public static defer_ defer { get; } = new ();
}
