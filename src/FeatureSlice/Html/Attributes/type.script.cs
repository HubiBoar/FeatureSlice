namespace FeatureSlice.Html;

public static partial class type
{
    public sealed record script(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("type", Value);

        public static script javascript { get; } = new ("text/javascript");
        public static script module { get; } = new ("module");
        public static script raw(string value) => new (value);
    }
}
