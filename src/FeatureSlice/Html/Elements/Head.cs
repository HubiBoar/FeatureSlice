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
        Title Title,
        Base Base,
        MetaList Meta,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, Base, Meta, Link, Style, Script);
    }

    public static Head Head()
    {
        return new Head(null, null, null, null, null, null);
    }

    public static Head Head
    (
        Title Title
    )
    {
        return new Head(Title, null, null, null, null, null);
    }

    public static Head Head
    (
        Base Base
    )
    {
        return new Head(null, Base, null, null, null, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base
    )
    {
        return new Head(Title, Base, null, null, null, null);
    }

    public static Head Head
    (
        MetaList Meta
    )
    {
        return new Head(null, null, Meta, null, null, null);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta
    )
    {
        return new Head(Title, null, Meta, null, null, null);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta
    )
    {
        return new Head(null, Base, Meta, null, null, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta
    )
    {
        return new Head(Title, Base, Meta, null, null, null);
    }

    public static Head Head
    (
        LinkList Link
    )
    {
        return new Head(null, null, null, Link, null, null);
    }

    public static Head Head
    (
        Title Title,
        LinkList Link
    )
    {
        return new Head(Title, null, null, Link, null, null);
    }

    public static Head Head
    (
        Base Base,
        LinkList Link
    )
    {
        return new Head(null, Base, null, Link, null, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        LinkList Link
    )
    {
        return new Head(Title, Base, null, Link, null, null);
    }

    public static Head Head
    (
        MetaList Meta,
        LinkList Link
    )
    {
        return new Head(null, null, Meta, Link, null, null);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        LinkList Link
    )
    {
        return new Head(Title, null, Meta, Link, null, null);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        LinkList Link
    )
    {
        return new Head(null, Base, Meta, Link, null, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta,
        LinkList Link
    )
    {
        return new Head(Title, Base, Meta, Link, null, null);
    }

    public static Head Head
    (
        StyleList Style
    )
    {
        return new Head(null, null, null, null, Style, null);
    }

    public static Head Head
    (
        Title Title,
        StyleList Style
    )
    {
        return new Head(Title, null, null, null, Style, null);
    }

    public static Head Head
    (
        Base Base,
        StyleList Style
    )
    {
        return new Head(null, Base, null, null, Style, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        StyleList Style
    )
    {
        return new Head(Title, Base, null, null, Style, null);
    }

    public static Head Head
    (
        MetaList Meta,
        StyleList Style
    )
    {
        return new Head(null, null, Meta, null, Style, null);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        StyleList Style
    )
    {
        return new Head(Title, null, Meta, null, Style, null);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        StyleList Style
    )
    {
        return new Head(null, Base, Meta, null, Style, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta,
        StyleList Style
    )
    {
        return new Head(Title, Base, Meta, null, Style, null);
    }

    public static Head Head
    (
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(null, null, null, Link, Style, null);
    }

    public static Head Head
    (
        Title Title,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(Title, null, null, Link, Style, null);
    }

    public static Head Head
    (
        Base Base,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(null, Base, null, Link, Style, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(Title, Base, null, Link, Style, null);
    }

    public static Head Head
    (
        MetaList Meta,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(null, null, Meta, Link, Style, null);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(Title, null, Meta, Link, Style, null);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(null, Base, Meta, Link, Style, null);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta,
        LinkList Link,
        StyleList Style
    )
    {
        return new Head(Title, Base, Meta, Link, Style, null);
    }

    public static Head Head
    (
        ScriptList Script
    )
    {
        return new Head(null, null, null, null, null, Script);
    }

    public static Head Head
    (
        Title Title,
        ScriptList Script
    )
    {
        return new Head(Title, null, null, null, null, Script);
    }

    public static Head Head
    (
        Base Base,
        ScriptList Script
    )
    {
        return new Head(null, Base, null, null, null, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        ScriptList Script
    )
    {
        return new Head(Title, Base, null, null, null, Script);
    }

    public static Head Head
    (
        MetaList Meta,
        ScriptList Script
    )
    {
        return new Head(null, null, Meta, null, null, Script);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        ScriptList Script
    )
    {
        return new Head(Title, null, Meta, null, null, Script);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        ScriptList Script
    )
    {
        return new Head(null, Base, Meta, null, null, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta,
        ScriptList Script
    )
    {
        return new Head(Title, Base, Meta, null, null, Script);
    }

    public static Head Head
    (
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(null, null, null, Link, null, Script);
    }

    public static Head Head
    (
        Title Title,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(Title, null, null, Link, null, Script);
    }

    public static Head Head
    (
        Base Base,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(null, Base, null, Link, null, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(Title, Base, null, Link, null, Script);
    }

    public static Head Head
    (
        MetaList Meta,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(null, null, Meta, Link, null, Script);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(Title, null, Meta, Link, null, Script);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(null, Base, Meta, Link, null, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta,
        LinkList Link,
        ScriptList Script
    )
    {
        return new Head(Title, Base, Meta, Link, null, Script);
    }

    public static Head Head
    (
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, null, null, null, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, null, null, null, Style, Script);
    }

    public static Head Head
    (
        Base Base,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, Base, null, null, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, Base, null, null, Style, Script);
    }

    public static Head Head
    (
        MetaList Meta,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, null, Meta, null, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, null, Meta, null, Style, Script);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, Base, Meta, null, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        MetaList Meta,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, Base, Meta, null, Style, Script);
    }

    public static Head Head
    (
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, null, null, Link, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, null, null, Link, Style, Script);
    }

    public static Head Head
    (
        Base Base,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, Base, null, Link, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        Base Base,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, Base, null, Link, Style, Script);
    }

    public static Head Head
    (
        MetaList Meta,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, null, Meta, Link, Style, Script);
    }

    public static Head Head
    (
        Title Title,
        MetaList Meta,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(Title, null, Meta, Link, Style, Script);
    }

    public static Head Head
    (
        Base Base,
        MetaList Meta,
        LinkList Link,
        StyleList Style,
        ScriptList Script
    )
    {
        return new Head(null, Base, Meta, Link, Style, Script);
    }
}
