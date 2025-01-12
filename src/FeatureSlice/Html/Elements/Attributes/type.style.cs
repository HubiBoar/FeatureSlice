namespace FeatureSlice.Html;

public static partial class type
{
    public sealed record style(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("type", Value);

        public static style css { get; } = new ("text/css");
        public static style x_scss { get; } = new ("text/x-scss");
        public static style x_less { get; } = new ("text/x-less");
        public static style raw(string value) => new (value);
    }
}
