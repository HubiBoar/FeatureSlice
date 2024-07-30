# FeatureSlice

[![Release Status](https://img.shields.io/github/actions/workflow/status/HubiBoar/FeatureSlice/publish.yml)](https://github.com/HubiBoar/FeatureSlice/actions/workflows/publish.yml)
[![NuGet Version](https://img.shields.io/nuget/v/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)

**FeatureSlice** is an library aiming to help working with Vertical/Feature Slice Architecture.

FeatureSlices contain a Handle method that can be invoked externaly by a delegate registered in DI.
The method can can be extended so it can be invoked by:
- Http Endpoint
- Queue/Topic
- Background Job
- CLI

Those elements can be setup using two types of API:

### [Samples](src/Samples/Samples.cs)

### Endpoint
```csharp
public sealed class ExampleHandler :
    FeatureSlice<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);

    public override ISetup Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
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
}
```

### CronJob
```csharp

public sealed class ExampleHandler :
    FeatureSlice<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);

    public override ISetup Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
    {
        Console.WriteLine($"Handler: {request}");

        await Task.CompletedTask;

        return new Response(request.Value2, request.Value1, request.Value0);
    })
    .MapCronJob
    (
        "5 4 * * *",
        new Request("testjob", 1, 2)
    );
}
```

### Consumer
```csharp
public sealed class ExampleConsumer :
    FeatureSlice<ExampleConsumer, ExampleConsumer.Request>
{
    public sealed record Request(string Value0, int Value1);

    public override ISetup Setup => Handle(static async (Request request, ExampleHandler.Dispatch dep2) => 
    {
        Console.WriteLine($"Consumer: {request}");

        await dep2(new ExampleHandler.Request("testFromConsumer", 0, 1));

        await Task.CompletedTask;

        return Result.Success;
    })
    .AsConsumer("ConsumerName");
}
```

### CLI
```csharp
public sealed class ExampleHandler :
    FeatureSlice<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public sealed record Request(string Value0, int Value1, int Value2);
    public sealed record Response(int Value0, int Value1, string Value2);

    public override ISetup Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
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
    );
}
```

### Combination of all of those
```csharp

public sealed class ExampleConsumer :
    FeatureSlice<ExampleConsumer, ExampleConsumer.Request>
{
    public sealed record Request(string Value0, int Value1);

    public override ISetup Setup => Handle(static async (Request request, ExampleHandler.Dispatch dep2) => 
    {
        Console.WriteLine($"Consumer: {request}");

        await dep2(new ExampleHandler.Request("testFromConsumer", 0, 1));

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
    );
}
```

### FeatureSlices Expose Dispatch methods which allow them to be called from dependencies
```csharp
public static void Use
(
    ExampleConsumer.Dispatch consumer,
    ExampleHandler.Dispatch handler
)
{
    consumer(new ExampleConsumer.Request());
    handler(new ExampleHandler.Request());
}
```

### FeatureSlices Expose Register for DI registration
```csharp
public static void Register(IServiceCollection services)
{
    ExampleConsumer.Register(services);
    ExampleHandler.Register(services);
}
```