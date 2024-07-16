using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();

public sealed class ExampleHandler :
    FeatureSlice2<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public record Request(string Value0, int Value1, int Value2);
    public record Response(int Value0, int Value1, string Value2);

    public override Options Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
    {
        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
    .MapPost("test", builder => builder
        .Request
        (
            Binder.FromRouteInt("id"),
            Binder.FromQueryInt("qu"),
            Binder.FromBody<Request>(),
            (id, qu, body) => new ("TestVal", qu, id)
        )
        .DefaultResponse()
        .WithTags("TestName"));
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