using System.Text;

namespace FeatureSlice.Html;

public static class HtmlHelper
{
    public static string Attribute(string name, string value)
    {
        return $"{name}=\"{value}\"";
    }

    public static string Simple
    (
        string name,
        IAttribute?[] attributes
    )
    {
        var attributesNotNull = attributes.Where(x => x is not null).Select(x => x!).ToArray();

        var attributesString = attributesNotNull.Length == 0 ? 
            string.Empty
            : 
            " " + string.Join
            (
                " ",
                attributesNotNull.Select(x => x.ToHtml())
            ); 

        return $"<{name}{attributesString}>";
    }

    public static string Collection
    (
        IReadOnlyCollection<IElement> elements
    )
    {
        return string.Join("\n", elements);
    }


    public static string Value
    (
        string name,
        string value
    )
    => Value(name, [], value);

    public static string Value
    (
        string name,
        IAttribute?[] attributes,
        string value
    )
    {
        var htmlName = Simple(name, attributes);

        return $"{htmlName}{value}</{name}>";
    }

    public static string Children
    (
        string name,
        IElement?[] children
    )
    => Children(name, [], children);

    public static string Children
    (
        string name,
        IAttribute?[] attributes,
        IElement?[] children
    )
    {
        var htmlName = Simple(name, attributes);

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(htmlName);

        foreach (var child in children.Where(x => x is not null))
        {
            var childHtml = child!.ToHtml();
            var childString = string.Join("\n\t", childHtml.Split('\n')); 

            stringBuilder.AppendLine(childString);
        }

        stringBuilder.AppendLine($"</{name}>");

        return stringBuilder.ToString();
    }
}

