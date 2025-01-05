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
    public static string Document(Head head, Body body)
    {
        return string.Empty;
    }

    public static Head Head()
    {
        return new Head(null, null);
    }

    public static Head Head(Title? title = null, Base? Base = null)
    {
        return new Head(title, Base);
    }

    public static Title Title(string title)
    {
        return new Title(title);
    }

    public static Base Base(Href href, Target? target = null)
    {
        return new Base(href, target);
    }

    public static Href Href(string url)
    {
        return new Href(url);
    }

    public static Target Target(Target.Type type)
    {
        return new Target(type);
    }

    public static Body Body()
    {
        return new Body();
    }
}
