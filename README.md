# FeatureSlice

[![Release Status](https://img.shields.io/github/actions/workflow/status/HubiBoar/FeatureSlice/publish.yml)](https://github.com/HubiBoar/FeatureSlice/actions/workflows/publish.yml)
[![NuGet Version](https://img.shields.io/nuget/v/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)

**FeatureSlice** is an library aiming to help working with Vertical/Feature Slice Architecture.

It achieves that by creating an abstraction in a form of two types of APIs:

### [FluentGenericBuilder](src/Samples/Builder.cs)

#### Endpoint
```csharp
public sealed class ExampleEndpoint : FeatureSliceBuilder
    .WithEndpoint
    .Build<ExampleEndpoint>
{
    protected override Endpoint Endpoint => Map.Get("test", (int age) => 
    {
        return Results.Ok();
    });
}
```

#### Handler
```csharp
public sealed class ExampleHandler : FeatureSliceBuilder
    .WithHandler<ExampleHandler.Request, ExampleHandler.Response, FromServices<Dependency1, Dependency2>>
    .Build<ExampleHandler>
{
    public record Request();
    public record Response();

    protected override async Task<Result<Response>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return new Response();
    }
}
```

#### Consumer
```csharp
public sealed class ExampleConsumer : FeatureSliceBuilder
    .WithConsumer<ExampleConsumer.Request, FromServices<Dependency1, Dependency2>>
    .Build<ExampleConsumer>
{
    public record Request();

    protected override ConsumerName ConsumerName => new("ExampleConsumer");

    protected override Task<Result> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        return Result.Success;
    }
}
```

#### Combination of Consumer with Endpoint, Handler with Endpoint and both with FeatureFlag
```csharp
public sealed class ExampleConsumerWithEndpoint : FeatureSliceBuilder
    .WithFlag
    .WithEndpoint
    .WithConsumer<ExampleConsumerWithEndpoint.Request, FromServices<Dependency1, Dependency2>>
    .Build<ExampleConsumerWithEndpoint>
{
    public record Request();

    protected override ConsumerName ConsumerName => new("ExampleConsumerWithEndpoint");

    protected override Endpoint Endpoint => Map.Get("test", (int age) => 
    {
        return Results.Ok();
    });

    protected override Task<Result> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        return Result.Success;
    }
}
```

#### Handlers and Consumers Expose Dispatch methods which allow them to be called from dependencies
```csharp
public static void Use(
    IPublisher publisher,
    ExampleConsumer.Dispatch consumer,
    ExampleHandler.Dispatch handler,
    ExampleConsumerWithEndpoint.Dispatch consumerWithEndpoint)
{
    publisher.Dispatch(new ExampleConsumer.Request());
    consumer(new ExampleConsumer.Request());
    handler(new ExampleHandler.Request());
    consumerWithEndpoint(new ExampleConsumerWithEndpoint.Request());
}
```

#### Handlers and Consumers Expose Register for DI registration
```csharp
public static void Register(IServiceCollection services, Messaging.ISetup setup, WebAppExtender hostExtender)
{
    ExampleEndpoint.Register(services, hostExtender);
    ExampleConsumer.Register(services, setup);
    ExampleHandler.Register(services, hostExtender);
    ExampleConsumerWithEndpoint.Register(services, setup, hostExtender);
}
```

## [Functional](src/Samples/Fluent.cs) 

### Samples
[src/Samples](src/Samples)

## License

The code in this repo is licensed under the [MIT](LICENSE) license.
