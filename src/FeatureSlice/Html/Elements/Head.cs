namespace FeatureSlice.Html;

public sealed record Head
(
    Title? Title, 
    Base? Base,
    MetaList? Meta,
    LinkList? Link,
    StyleList? Style,
    ScriptList? Script
)
: IElement
{
    public string ToHtml() => HtmlHelper.Children("head", [ Title, Base, Meta, Link, Style, Script]);

    private static void GenerateAllCombinations()
    {
        for (int i = 0; i < 64; i++) // 2^6 = 64 combinations
        {
            (string? Name, string Call) title = (i & 1) != 0 ? ("Title Title", "Title") : (null, "null");       // Bit 0
            (string? Name, string Call) @base = (i & 2) != 0 ? ("Base Base", "Base") : (null, "null");        // Bit 1
            (string? Name, string Call) meta = (i & 4) != 0 ? ("MetaList Meta", "Meta") : (null, "null");     // Bit 2
            (string? Name, string Call) link = (i & 8) != 0 ? ("LinkList Link", "Link") : (null, "null");      // Bit 3
            (string? Name, string Call) style = (i & 16) != 0 ? ("StyleList Style", "Style") : (null, "null");    // Bit 4
            (string? Name, string Call) script = (i & 32) != 0 ? ("ScriptList Script", "Script") : (null, "null");   // Bit 5

            var names = new string?[] { title.Name, @base.Name, meta.Name, link.Name, style.Name, script.Name };
            var calls = new string[] { title.Call, @base.Call, meta.Call, link.Call, style.Call, script.Call };
            string namesString = string.Join(",\n\t", names.Where(x => x is not null));
            string callsString = string.Join(", ", calls);
            Console.WriteLine
            (
            $$"""

            public static Head Head
            (
                {{namesString}}
            )
            {
                return new Head({{callsString}});
            }
            """
            );
        }
    }
}

public static partial class HTML
{
    public static Head Head
    (
        Title? Title = null,
        Base? Base = null,
        MetaList? Meta = null,
        LinkList? Link = null,
        StyleList? Style = null,
        ScriptList? Script = null
    )
    {
        return new Head(Title, Base, Meta, Link, Style, Script);
    }
}
