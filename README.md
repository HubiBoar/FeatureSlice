# FeatureSlice

[![Release Status](https://img.shields.io/github/actions/workflow/status/HubiBoar/FeatureSlice/publish.yml)](https://github.com/HubiBoar/FeatureSlice/actions/workflows/publish.yml)
[![NuGet Version](https://img.shields.io/nuget/v/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)

**FeatureSlice** is an library aiming to help working with Vertical/Feature Slice Architecture.

FeatureSlices contain a public Dispatch delegate property that can be invoked by dependencies.
The method can can be extended so it can be invoked by:
- Http Endpoint
- Queue/Topic
- Background Job
- CLI

### [Samples](src/Samples/Sample.cs)

### Endpoint
```csharp
public sealed record ExampleHandler() : FeatureSlice<ExampleHandler.Request, ExampleHandler.Response>
(
    Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) =>
    {
        Console.WriteLine($"Handler: {request}");

        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
    .MapPost("handler", opt => opt
        .Request
        (
            From.Route.Int("id"),
            From.Query.Int("qu"),
            From.Body.Json<Request>(),
            (id, qu, body) => new Request(body.Value0, qu, id)
        )
        .DefaultResponse()
        .WithTags("Handler"))
)
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);
}
```

### CronJob
```csharp

public sealed record ExampleHandler() : FeatureSlice<ExampleHandler.Request, ExampleHandler.Response>
(
    Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) =>
    {
        Console.WriteLine($"Handler: {request}");

        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
    .MapCronJob
    (
        "5 4 * * *",
        new Request("testjob", 1, 2)
    )
)
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);
}
```

### Consumer
Which requires a QueueName that this handler will subscribe to.
```csharp
public sealed record ExampleConsumer() : FeatureSlice<ExampleConsumer.Request>
(
    Handle(static async (Request request, Dependency1 dep1, ExampleHandler dep2) => 
    {
        Console.WriteLine($"Consumer: {request}");

        await dep2.Dispatch(new ExampleHandler.Request("testFromConsumer", 0, 1));

        await Task.CompletedTask;

        return Result.Success;
    })
    .AsConsumer("ConsumerName")
}
```

### CLI
```csharp
public sealed record ExampleHandler() : FeatureSlice<ExampleHandler.Request, ExampleHandler.Response>
(
    Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) =>
    {
        Console.WriteLine($"Handler: {request}");

        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
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
```

### Combination of all of those
```csharp

public sealed record ExampleConsumer() : FeatureSlice<ExampleConsumer.Request>
(
    Handle(static async (Request request, Dependency1 dep1, ExampleHandler dep2) => 
    {
        Console.WriteLine($"Consumer: {request}");

        await dep2.Dispatch(new ExampleHandler.Request("testFromConsumer", 0, 1));

        await Task.CompletedTask;

        return Result.Success;
    })
    .MapPost("consumer", opt => opt
        .Request
        (
            From.Route.Int("id"),
            From.Body.Json<Request>(),
            (id, body) => new Request(body.Value0, id)
        )
        .DefaultResponse()
        .WithTags("Consumer"))
    .AsConsumer("ConsumerName")
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
```

### FeatureSlices Expose Dispatch methods which allow them to be called from dependencies
```csharp
public static void Use
(
    ExampleConsumer consumer,
    ExampleHandler handler
)
{
    consumer.Dispatch(new ExampleConsumer.Request("testConsumer", 1));
    handler.Dispatch(new ExampleHandler.Request("testHandler", 2, 3));
}
```

### FeatureSlices Expose Register for DI registration
```csharp
public static void Register(IServiceCollection services, string[] args)
{
    services.AddFeatureSlices()
        .DefaultConsumerDispatcher()
        .DefaultDispatcher()
        .MapCli(args);

    services.AddFeatureSlice<ExampleHandler>();
    services.AddFeatureSlice<ExampleConsumer>();
}
```

### FeatureSlices can be easly mocked without the need for interfaces
```csharp
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
```
