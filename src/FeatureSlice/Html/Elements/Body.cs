namespace FeatureSlice.Html;

public sealed class Body : IElement
{
    public string ToHtml() => throw new NotImplementedException();
}

public static partial class HTML
{
    public static Body Body()
    {
        return new Body();
    }
}
