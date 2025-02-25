using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Handle2;

public sealed record Dep0;
public sealed record Dep1;

public partial record Example() : FeatureSlice<Example.Request, Example.Response>
(
    Handle(static (Request request, Dep0 dep0, Dep1 dep1) => 
    {
        return new Response();
    })
    .Route(HttpMethod.Get, "/route") 
        .Html(request => Html.Empty.Htmx<Example, Request>())
)
{
    public sealed record Request();
    public sealed record Response();
}

public static class Test
{
    public static void Run(Example example)
    {
        example.Dispatch(new Example.Request());
    }

    public static void Register(IServiceCollection services)
    {
        Example.Register(services);
    }

    public static void Tests()
    {
        var example = new Example()
        {
            Dispatch = (request) => null!
        };
    }
}
