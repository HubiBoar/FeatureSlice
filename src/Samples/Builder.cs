using Microsoft.AspNetCore.Mvc;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();

public sealed class ExampleHandler :
    FeatureSlice2<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public record Request(string Value1, int Value2);
    public record Response(int Value1, string Value2);

    public override Options Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
    {
        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1);
    })
    .MapPost("test")
        .Request()
        .FromRoute<int, IRouteName.Id>()
        .DefaultResult(route => new ("TestVal", route))
        //.Setup(([FromRoute] string id, [FromBody] Request request) => request)
        //.FromBody()
        //.DefaultResult()
    .WithTags("TestName");
}

public class Example
{
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<Dependency1>();
        services.AddSingleton<Dependency2>();
        ExampleHandler.Register(services);
    }
}