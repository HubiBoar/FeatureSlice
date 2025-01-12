namespace FeatureSlice.Html;

public static partial class Attribute
{
    public static class Script
    {
        public sealed record Type(string Value) : IAttribute
        {
            public string ToHtml() => HtmlHelper.Attribute("type", Value);
        }
    }

    public sealed record Async() : IAttribute
    {
        public string ToHtml() => "async";
    }

    public sealed record Defer() : IAttribute
    {
        public string ToHtml() => "defer";
    }

    public sealed record Nomodule() : IAttribute
    {
        public string ToHtml() => "nomodule";
    }

    public sealed record Src(string Value) : IAttribute
    {
        public string ToHtml() => HtmlHelper.Attribute("src", Value);
    }
}

public interface IScript : IElement;

public sealed record ScriptSrc
(
    Attribute.Src Src,
    Attribute.Script.Type? Type,
    Attribute.Async? Async,
    Attribute.Defer? Defer,
    Attribute.Nomodule? Nomodule,
    Attribute.Integrity? Integrity,
    Attribute.CrossOrigin? Crossorigin
)
: IScript
{
    public string ToHtml() => HtmlHelper.Simple("script", [ Src, Type, Async, Defer, Nomodule, Integrity, Crossorigin ]);
}

public sealed record Script
(
    string Value,
    Attribute.Script.Type? Type,
    Attribute.Async? Async,
    Attribute.Defer? Defer,
    Attribute.Integrity? Integrity,
    Attribute.CrossOrigin? Crossorigin
)
: IScript
{
    public static class type
    {
        public static Attribute.Script.Type javascript { get; } = new ("text/javascript");
        public static Attribute.Script.Type module { get; } = new ("module");
        public static Attribute.Script.Type raw(string value) => new (value);
    }

    public string ToHtml() => HtmlHelper.Value
    (
        "script",
        [ Type, Async, Defer, Integrity, Crossorigin ],
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
        Attribute.Src src,
        Attribute.Script.Type? type = null,
        Attribute.Async? @async = null,
        Attribute.Defer? defer = null,
        Attribute.Nomodule? nomodule = null,
        Attribute.Integrity? integrity = null,
        Attribute.CrossOrigin? crossorigin = null
    )
    {
        return new ScriptSrc(src, type, @async, defer, nomodule, integrity, crossorigin);
    }

    public static Func<string, Script> Script
    (
        Attribute.Script.Type? type = null,
        Attribute.Async? @async = null,
        Attribute.Defer? defer = null,
        Attribute.Integrity? integrity = null,
        Attribute.CrossOrigin? crossorigin = null
    )
    {
        return script => new Script(script, type, @async, defer, integrity, crossorigin);
    }

    public static Attribute.Async @async { get; } = new ();
    public static Attribute.Defer defer { get; } = new ();
    public static Attribute.Nomodule nomodule { get; } = new ();
    public static Attribute.Src src(string value) => new (value);

    public static ScriptList Script
    (
        params FeatureSlice.Html.IScript[] list
    )
    {
        return new ScriptList(list);  
    }
}
