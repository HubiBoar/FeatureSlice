namespace FeatureSlice.Html;

public static partial class Attribute
{
    public sealed record Target(Target.Type Value) : IAttribute
    {
        public enum Type
        {
            Self,
            Blank,
            Parent,
            Top
        }

        public string ToHtml() => HtmlHelper.Attribute("target", Value switch
        {
            Type.Self => "_self",
            Type.Blank => "_blank",
            Type.Parent => "_parent",
            Type.Top => "_top",
            _ => throw new ArgumentOutOfRangeException(nameof(Target.Value), Value, null)
        });
    }
}

public static partial class HTML
{
    public static class target
    {
        public static Attribute.Target self { get; }   = new Attribute.Target(Attribute.Target.Type.Self);
        public static Attribute.Target blank { get; }  = new Attribute.Target(Attribute.Target.Type.Blank);
        public static Attribute.Target parent { get; } = new Attribute.Target(Attribute.Target.Type.Parent);
        public static Attribute.Target top { get; }    = new Attribute.Target(Attribute.Target.Type.Top);
    }
}
