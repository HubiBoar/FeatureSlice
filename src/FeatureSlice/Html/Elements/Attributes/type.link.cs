namespace FeatureSlice.Html;

public static partial class type
{
    public sealed record link(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("rel", Value);

        public static type.link raw(string value) => new (value);
    }
}
