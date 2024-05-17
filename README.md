# FeatureSlice

[![Release Status](https://img.shields.io/github/actions/workflow/status/HubiBoar/FeatureSlice/publish.yml)](https://github.com/HubiBoar/FeatureSlice/actions/workflows/publish.yml)
[![NuGet Version](https://img.shields.io/nuget/v/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FeatureSlice)](https://www.nuget.org/packages/FeatureSlice/)

**FeatureSlice** is an library aiming to help working with Vertical/Feature Slice Architecture.
FeatureSlices can be Endpoints, Handlers and Consumers, and can also have a FeatureToggle.
- Handlers are a InMemory classes which take and Request and produces an Response.
- Consumers are Message Consumers which take an Request and produces only Success or Error status, they can be setup to use for example Azure ServiceBus.
- Both of those can also be reachable from API as Endpoints and/or be toggable using FeatureToggles. 

Those elements can be setup using two types of API:

## [FluentGenericBuilder](src/Samples/Builder.cs)

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

## [Functional](src/Samples/Fluent.cs) which needs more manual setup

#### Endpoint
```csharp
public static class ExampleEndpoint
{
    public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
    {
        services.FeatureSlice()
            .WithEndpoint(
                extender,
                Endpoint);
    }

    private static Endpoint Endpoint => Map.Get("test", (int age) => 
    {
        return Results.Ok();
    });
}
```

#### Handler
```csharp
public sealed class ExampleHandler : Dispatchable<ExampleHandler, ExampleHandler.Request, Result<ExampleHandler.Response, Disabled>>
{
    public record Request();
    public record Response();

    public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
    {
        services.FeatureSlice()
            .WithHandler<Dispatch, Request, Response, FromServices<Dependency1, Dependency2>>(
                Handle,
                handler => handler.Invoke);
    }

    private static async Task<Result<Response>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return new Response();
    }
}
```

#### Consumer
```csharp
public static class ExampleConsumer
{
    public delegate Task<Result> Dispatch(Request request);

    public record Request();

    public static void Register(IServiceCollection services, Messaging.ISetup setup)
    {
        services.FeatureSlice()
            .WithConsumer<Dispatch, Request, FromServices<Dependency1, Dependency2>>(
                setup,
                new ("ExampleConsumer"),
                Consume,
                handler => handler.Invoke);
    }

    private static async Task<Result> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return Result.Success;
    }
}
```

#### Combination of Consumer with Endpoint, Handler with Endpoint and both with FeatureFlag
```csharp
public static class ExampleConsumerWithEndpoint
{
    public delegate Task<Result.Or<Disabled>> Dispatch(Request request);

    public record Request();

    public static void Register(IServiceCollection services, Messaging.ISetup setup, IHostExtender<WebApplication> extender)
    {
        services.FeatureSlice()
            .WithFlag("ExampleConsumerWithEndpoint")
            .WithEndpoint(
                extender,
                Endpoint)
            .WithConsumer<Dispatch, Request, FromServices<Dependency1, Dependency2>>(
                setup,
                new ("ExampleConsumerWithEndpoint"),
                Consume,
                handler => handler.Invoke);
    }

    private static Endpoint Endpoint => Map.Get("test", (int age) => 
    {
        return Results.Ok();
    });

    private static async Task<Result> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return Result.Success;
    }
}
```

#### Handlers and Consumers need to have manually added Dispatch methods which allow them to be called from dependencies
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

#### Handlers and Consumers need to have manually added Register for DI registration
```csharp
public static void Register(IServiceCollection services, Messaging.ISetup setup, WebAppExtender hostExtender)
{
    ExampleEndpoint.Register(services, hostExtender);
    ExampleConsumer.Register(services, setup);
    ExampleHandler.Register(services, hostExtender);
    ExampleConsumerWithEndpoint.Register(services, setup, hostExtender);
}
```

### Samples
[src/Samples](src/Samples)

## License

The code in this repo is licensed under the [MIT](LICENSE) license.
