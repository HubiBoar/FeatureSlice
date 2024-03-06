using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Momolith.Modules;
using OneOf;
using OneOf.Types;
using Definit.Dependencies;
using Definit.Endpoint;
using Endpoint = Definit.Endpoint.Endpoint;

namespace FeatureSlice.Samples.Fluent;

public sealed record Dependency1();
public sealed record Dependency2();

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

public sealed class ExampleHandler : DispatchableWithFlag<ExampleHandler, ExampleHandler.Request, ExampleHandler.Response>
{
    public record Request();
    public record Response();

    public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
    {
        services.FeatureSlice()
            .WithFlag("ExampleHandler")
            .WithEndpoint(
                extender,
                Endpoint)
            .WithHandler<Dispatch, Request, Response, FromServices<Dependency1, Dependency2>>(
                Handle,
                handler => handler.Invoke);
    }

    private static Endpoint Endpoint => Map.Get("test", (int age) => 
    {
        return Results.Ok();
    });

    private static async Task<OneOf<Response, Error>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        return new Response();
    }
}

public static class ExampleConsumer
{
    public delegate Task<OneOf<Success, Disabled, Error>> Dispatch(Request request);

    public record Request();

    public static void Register(IServiceCollection services, Messaging.ISetup setup)
    {
        services.FeatureSlice()
            .WithFlag("ExampleConsumer")
            .WithConsumer<Dispatch, Request, FromServices<Dependency1, Dependency2>>(
                setup,
                new ("ExampleConsumer"),
                Consume,
                handler => handler.Invoke);
    }

    private static async Task<OneOf<Success, Error>> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        return new Success();
    }
}

public static class ExampleConsumerWithEndpoint
{
    public delegate Task<OneOf<Success, Disabled, Error>> Dispatch(Request request);

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

    private static async Task<OneOf<Success, Error>> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        return new Success();
    }
}

public class Usage
{
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

    public static void Register(IServiceCollection services, Messaging.ISetup setup, WebAppExtender hostExtender)
    {
        ExampleEndpoint.Register(services, hostExtender);
        ExampleConsumer.Register(services, setup);
        ExampleHandler.Register(services, hostExtender);
        ExampleConsumerWithEndpoint.Register(services, setup, hostExtender);
    }
}