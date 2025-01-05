using System.Text;

namespace FeatureSlice.Html;

public interface IAttribute
{
    public string _Name { get; }
    string _Value { get; }
}

public static partial class Attribute
{
    public sealed record Href(string _Value) : IAttribute
    {
        public string _Name { get; } = "href";
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

        public string _Name { get; } = "target";

        public string _Value { get; } = Value switch
        {
            Type.Self => "_self",
            Type.Blank => "_blank",
            Type.Parent => "_parent",
            Type.Top => "_top",
            _ => throw new ArgumentOutOfRangeException(nameof(Target.Value), Value, null)
        };
    }
}

public static class HtmlHelper
{
    public static string Name(IElement element)
    {
        var attributes = element._Attributes;
        var name = element._Name;

        var attributesString = attributes.Count == 0 ? string.Empty : " " + string.Join(string.Empty, attributes.Select(x => $"""{x._Name}="{x._Value}" """)); 

        return $"<{name}{attributesString}>";
    }

    public static string ToHtml(this IElement element) => element.ToHtml();
}

public interface IElement
{
    public string _Name { get; }

    public IReadOnlyList<IAttribute> _Attributes { get; }

    public static IReadOnlyList<IAttribute> Get(params IAttribute?[] values)
    {
        return values.Where(x => x is not null).Select(x => x!).ToList();
    }

    public static IReadOnlyList<IElement> Get(params IElement?[] values)
    {
        return values.Where(x => x is not null).Select(x => x!).ToList();
    }

    public virtual string ParseToHtml()
    {
        return HtmlHelper.Name(this);
    }
}

public interface IElementWithValue : IElement
{
    public string _Value { get; } 

    string IElement.ParseToHtml()
    {
        var attributes = _Attributes;
        var value = _Value;
        var name = _Name;

        var stringBuilder = new StringBuilder();

        var htmlName = HtmlHelper.Name(this);

        return $"{htmlName}{value}</{name}>";
    }
}

public interface IElementWithChildren : IElement
{
    public IReadOnlyList<IElement> _Children { get; }

    string IElement.ParseToHtml()
    {
        var attributes = _Attributes;
        var children = _Children;
        var name = _Name;

        var stringBuilder = new StringBuilder();

        var htmlName = HtmlHelper.Name(this);

        stringBuilder.AppendLine(htmlName);

        foreach (var child in children)
        {
            var childHtml = child.ToHtml();
            var childString = string.Join("\n\t", childHtml.Split('\n')); 

            stringBuilder.AppendLine(childString);
        }

        stringBuilder.AppendLine($"</{name}>");

        return stringBuilder.ToString();
    }
}

public sealed record Document(Head? Head, Body? Body) : IElementWithChildren
{
    public string _Name { get; } = "html";

    public IReadOnlyList<IAttribute> _Attributes { get; } = [];

    public IReadOnlyList<IElement> _Children { get; } = IElement.Get(Head, Body);
}

public sealed record Head(Title? Title, Base? Base)  : IElementWithChildren
{
    public string _Name { get; } = "head";

    public IReadOnlyList<IAttribute> _Attributes { get; } = [];

    public IReadOnlyList<IElement> _Children { get; } = IElement.Get(Title, Base);
}

public sealed record Title(string Value) : IElementWithChildren
{
    public string _Name { get; } = "title";

    public IReadOnlyList<IAttribute> _Attributes { get; } = [];

    public IReadOnlyList<IElement> _Children { get; } = IElement.Get(Value);
}

public sealed record Base(Attribute.Href Href, Attribute.Target? Target) : IElement
{
    public static string Name { get; } = "base";
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
        public static Attribute.Target self { get; }   = new Attribute.Target(Attribute.Target.Type.Self);
        public static Attribute.Target blank { get; }  = new Attribute.Target(Attribute.Target.Type.Blank);
        public static Attribute.Target parent { get; } = new Attribute.Target(Attribute.Target.Type.Parent);
        public static Attribute.Target top { get; }    = new Attribute.Target(Attribute.Target.Type.Top);
    }

    public static string Document(Head? Head = null, Body? Body = null)
    {
        return new Document(Head, Body).ToHtml();
    }

    public static Head Head(Title? Title = null, Base? Base = null, MetaList? Meta = null)
    {
        return new Head(Title, Base);
    }

    public static Title Title(string title)
    {
        return new Title(title);
    }

    public static Base Base(Attribute.Href href, Attribute.Target? target = null)
    {
        return new Base(href, target);
    }

    public static Attribute.Href href(string url)
    {
        return new Attribute.Href(url);
    }

    public static Body Body()
    {
        return new Body();
    }
}
