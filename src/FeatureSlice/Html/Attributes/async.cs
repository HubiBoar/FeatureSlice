namespace FeatureSlice.Html;

public sealed record async_() : IAttribute
{
    public string ToHtml() => "async";
}

public static partial class HTML
{
    public static async_ async { get; } = new ();
}
