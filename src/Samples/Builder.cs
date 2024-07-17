using Definit.Results;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();

public sealed class ExampleHandler :
    FeatureSlice<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);

    public override Options Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
    {
        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
    .MapPost("handler", builder => builder
        .Request
        (
            From.Route.Int("id"),
            From.Query.Int("qu"),
            From.Body.Json<Request>(),
            (id, qu, body) => new (body.Value0, qu, id)
        )
        .DefaultResponse()
        .WithTags("Handler"));
}

public sealed class ExampleConsumer :
    FeatureSlice<ExampleConsumer, ExampleConsumer.Request>
{
    public sealed record Request(string Value0, int Value1);

    public override Options Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
    {
        await Task.CompletedTask;

        return Result.Success;
    })
    .MapPost("consumer", builder => builder
        .Request
        (
            From.Route.Int("id"),
            From.Body.Json<Request>(),
            (id, body) => new (body.Value0, id)
        )
        .DefaultResponse()
        .WithTags("Consumer"))
    .AsConsumer();
}

public class Example
{
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<Dependency1>();
        services.AddSingleton<Dependency2>();
        ExampleHandler.Register(services);
        ExampleConsumer.Register(services);
    }
}