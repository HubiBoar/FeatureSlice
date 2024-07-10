using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Momolith.Modules;
using Definit.Dependencies;
using Definit.Endpoint;
using Endpoint = Definit.Endpoint.Endpoint;
using Definit.Results;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();

public sealed class ExampleHandler : FeatureSliceBuilder
    .Handler<ExampleHandler.Request, ExampleHandler.Response, FromServices<Dependency1, Dependency2>>
    .WithFlag
    .WithEndpoint
    .Build<ExampleHandler>
{
    public record Request();
    public record Response();

    protected override string FeatureName => "ExampleHandler";

    protected override Endpoint Endpoint => Map.Get("test", (int age, Dispatch dispatch) => 
    {
        return Results.Ok();
    })
    .WithDescription("Name");

    protected override async Task<Result<Response>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return new Response();
    }
}

public sealed class ExampleConsumer : FeatureSliceBuilder
    .Consumer<ExampleConsumer.Request, FromServices<Dependency1, Dependency2>>
    .WithFlag
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

public sealed class ExampleConsumerWithEndpoint : FeatureSliceBuilder
    .Consumer<ExampleConsumerWithEndpoint.Request, FromServices<Dependency1, Dependency2>>
    .WithFlag
    .WithEndpoint
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
        ExampleConsumer.Register(services, setup);
        ExampleHandler.Register(services, hostExtender);
        ExampleConsumerWithEndpoint.Register(services, setup, hostExtender);
    }
}