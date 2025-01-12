namespace FeatureSlice.Html;

public sealed record Document(Head? Head, Body? Body) : IElement
{
    public string ToHtml() => HtmlHelper.Children("html", [ Head, Body ]);
}

public static partial class HTML
{
    public static string Document(Head? Head = null, Body? Body = null)
    {
        return new Document(Head, Body).ToHtml();
    }
}
