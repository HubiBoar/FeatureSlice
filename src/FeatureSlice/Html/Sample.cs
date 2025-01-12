using static FeatureSlice.Html.HTML;

namespace FeatureSlice.Html;

internal static class Sample
{
    public static string Test() =>
    Document
    (
        Head
        (
            Title("test"),
            Base(href("https://www.example.com/"), target.self),
            Meta
            (
                Meta(charset.UTF_8),
                Meta(name("name"), content("content")),
                Meta(http_equiv("name"), content("content"))
            ),
            Link
            (
                Link(href("href"), rel("test"), sizes: sizes("sizes")),
                Link(href("href"), rel("test"))
            ),
            Style(Style.type.css)
            (
                "css"
            ),
            Script
            (
                Script(Script.type.javascript)
                (
                    """console.log("Inline script executed.");"""
                ),
                Script
                (
                    src("javascript.js"),
                    @async: @async,
                    defer: defer
                )
            )
        ),
        Body()
    );
    //<html>
    //  <head>
    //    <title> test </title>
    //    <base href="https://www.example.com/" target="_self">
    //  </head>
    //  <meta>
    //  <link href="href" rel="rel" sizes="sizes>
    //  <link href="href" rel="rel">
    //  <style type="css>
    //      css
    //  </style>
    //  <body>
    //  </body>
    //</html>  
}
