namespace FeatureSlice.Html;

public interface IAttribute
{
    public static abstract string Name { get; }
}

public sealed record Href(string Url) : IAttribute
{
    public static string Name { get; } = "href";
}

public sealed record Target(Target.Type Value) : IAttribute
{
    public enum Type
    {
        Self,
        Blank,
        Parent,
        Top
    }

    public static string Name { get; } = "target";
}


public interface IElement
{
    public static abstract string Name { get; }
}

public sealed record Document(Head? Head, Body? Body) : IElement
{
    public static string Name { get; } = "html";

    public string ToHtml() => string.Empty;
}

public sealed record Head(Title? Title, Base? Base)  : IElement
{
    public static string Name { get; } = "head";
}

public sealed record Title(string Value) : IElement
{
    public static string Name { get; } = "title";
}

public sealed record Base(Href Href, Target? Target) : IElement
{
    public static string Name { get; } = "base";
}

public sealed record Meta() : IElement
{
    public static string Name { get; } = "meta";
}

public sealed record Link() : IElement
{
    public static string Name { get; } = "link";
}

public sealed record Style() : IElement
{
    public static string Name { get; } = "style";
}

public sealed record Script() : IElement
{
    public static string Name { get; } = "script";
}

public sealed class Body : IElement
{
    public static string Name { get; } = "body";
}

public static partial class HTML
{
    public static class target
    {
        public static Target self { get; }   = new Target(Target.Type.Self);
        public static Target blank { get; }  = new Target(Target.Type.Blank);
        public static Target parent { get; } = new Target(Target.Type.Parent);
        public static Target top { get; }    = new Target(Target.Type.Top);
    }

    public static string Document(Head? Head = null, Body? Body = null)
    {
        return new Document(Head, Body).ToHtml();
    }

    public static Head Head(Title? Title = null, Base? Base = null)
    {
        return new Head(Title, Base);
    }

    public static Title Title(string title)
    {
        return new Title(title);
    }

    public static Base Base(Href href, Target? target = null)
    {
        return new Base(href, target);
    }

    public static Href href(string url)
    {
        return new Href(url);
    }

    public static Body Body()
    {
        return new Body();
    }
}
