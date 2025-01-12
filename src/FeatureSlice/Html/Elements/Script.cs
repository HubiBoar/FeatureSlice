namespace FeatureSlice.Html;

public interface IScript : IElement;

public sealed record ScriptSrc
(
    src src,
    type.script? type,
    async_? async,
    defer_? defer,
    nomodule_? nomodule,
    integrity? integrity,
    crossorigin? crossorigin
)
: IScript
{
    public string ToHtml() => HtmlHelper.Simple("script", [ src, type, async, defer, nomodule, integrity, crossorigin ]);
}

public sealed record Script
(
    string Value,
    type.script? type,
    async_? async,
    defer_? defer,
    integrity? integrity,
    crossorigin? crossorigin
)
: IScript
{
    public string ToHtml() => HtmlHelper.Value
    (
        "script",
        [ type, async, defer, integrity, crossorigin ],
        Value
    );
}

public sealed record ScriptList(IReadOnlyCollection<IScript> Collection) : IElement
{
    public string ToHtml() => HtmlHelper.Collection(Collection);
}

public static partial class HTML
{
    public static ScriptSrc Script
    (
        src src,
        type.script? type = null,
        async_? async = null,
        defer_? defer = null,
        nomodule_? nomodule = null,
        integrity? integrity = null,
        crossorigin? crossorigin = null
    )
    {
        return new ScriptSrc(src, type, @async, defer, nomodule, integrity, crossorigin);
    }

    public static Func<string, Script> Script
    (
        type.script? type = null,
        async_? async = null,
        defer_? defer = null,
        integrity? integrity = null,
        crossorigin? crossorigin = null
    )
    {
        return script => new Script(script, type, @async, defer, integrity, crossorigin);
    }

    public static ScriptList Script
    (
        params FeatureSlice.Html.IScript[] list
    )
    {
        return new ScriptList(list);  
    }
}
