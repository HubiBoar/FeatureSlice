using Definit.Results;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();

public sealed record ExampleHandler() : FeatureSlice<ExampleHandler.Request, ExampleHandler.Response>
(
    Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) =>
    {
        Console.WriteLine($"Handler: {request}");

        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
    .MapPost("handler", static opt => opt
        .Request
        (
            From.Route.Int("id"),
            From.Query.Int("qu"),
            From.Body.Json<Request>(),
            (id, qu, body) => new (body.Value0, qu, id)
        )
        .DefaultResponse()
        .WithTags("Handler"))
    .MapCronJob
    (
        "5 4 * * *",
        new Request("testjob", 1, 2)
    )
    .MapCli
    (
        Arg.Cmd("validate"),
        Arg.Opt("option1", "o1"),
        Arg.Opt("option2", "o2"),
        (arg1, arg2) => new Request(arg1, int.Parse(arg2), 5)
    )
)
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);
}

public sealed record ExampleConsumer() : FeatureSlice<ExampleConsumer.Request>
(
    Handle(static async (Request request, Dependency1 dep1, ExampleHandler dep2) => 
    {
        Console.WriteLine($"Consumer: {request}");

        await dep2.Dispatch(new ExampleHandler.Request("testFromConsumer", 0, 1));

        await Task.CompletedTask;

        return Result.Success;
    })
    .MapPost("consumer", static opt => opt
        .Request
        (
            From.Route.Int("id"),
            From.Body.Json<Request>(),
            (id, body) => new (body.Value0, id)
        )
        .DefaultResponse()
        .WithTags("Consumer"))
    .AsConsumer("ConsumerName")
)
{
    public sealed record Request(string Value0, int Value1);
}

public class Example
{
    public static void Use
    (
        ExampleConsumer consumer,
        ExampleHandler handler
    )
    {
        consumer.Dispatch(new ExampleConsumer.Request("testConsumer", 1));
        handler.Dispatch(new ExampleHandler.Request("testHandler", 2, 3));
    }

    public static void Register(IServiceCollection services, string[] args)
    {
        services.AddSingleton<Dependency1>();
        services.AddSingleton<Dependency2>();

        services.AddFeatureSlices()
            .DefaultConsumerDispatcher()
            .DefaultDispatcher()
            .MapCli(args);

        services.AddFeatureSlice<ExampleHandler>();
        services.AddFeatureSlice<ExampleConsumer>();
    }

    public static void TestMocking()
    {
        var consumerMock = new ExampleConsumer()
        {
            Dispatch = request => Result.Success
        };

        var handlerMock = new ExampleHandler()
        {
            Dispatch = async request => new ExampleHandler.Response(1, 2, "testResponse")
        };

        Use(consumerMock, handlerMock);
    }
}
