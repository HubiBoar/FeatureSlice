namespace FeatureSlice.Html;

public sealed record target(target.Type Value) : IAttribute
{
    public static target self { get; }   = new (Type.Self);
    public static target blank { get; }  = new (Type.Blank);
    public static target parent { get; } = new (Type.Parent);
    public static target top { get; }    = new (Type.Top);

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
        _ => throw new ArgumentOutOfRangeException(nameof(target.Value), Value, null)
    });
}
