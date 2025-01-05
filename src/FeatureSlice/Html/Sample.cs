using static FeatureSlice.Html.HTML;

namespace FeatureSlice.Html;

internal static class Sample
{
    public static string Test() =>
    Document
    (
        Head(title: Title("test"), Base: Base(Href("https://www.example.com/"), Target(Target.Type.Self))),
        //<head>
        //  <title> test </title>
        //  <base href="https://www.example.com/" target="_self">
        //</head>
        //   
        Body()
    );
}
