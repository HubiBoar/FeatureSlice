namespace FeatureSlice.Html;

public sealed record nomodule_() : IAttribute
{
    public string ToHtml() => "nomodule";
}

public static partial class HTML
{
    public static nomodule_ nomodule { get; } = new ();
}
