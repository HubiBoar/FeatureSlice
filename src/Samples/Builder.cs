using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Momolith.Modules;
using Definit.Dependencies;
using Definit.Endpoint;
using Endpoint = Definit.Endpoint.Endpoint;
using Definit.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();



public sealed class ExampleHandler2 :
    FeatureSlice<ExampleHandler2.Request, ExampleHandler2.Response, FromServices<Dependency1, Dependency2>>
    .WithEndpoint
    .Build<ExampleHandler2>
{
    public record Request();
    public record Response();

    protected override Endpoint Endpoint => Map.Get("test", (int age, Handle handle) => 
    {
        return Results.Ok();
    })
    .WithDescription("Name");

    protected override async Task<Result<Response>> OnRequest(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return new Response();
    }
}

public sealed class ExampleHandlerAlt :
    FeatureSlice<ExampleHandlerAlt.Request, ExampleHandlerAlt.Response, FromServices<Dependency1, Dependency2>>
    .WithEndpoint<IHttpMethod.Get>
    .Build<ExampleHandlerAlt>
{
    public record Request();
    public record Response();

    protected override string Path => "test";
    protected override void AppendEndpoint(IEndpointConventionBuilder builder)
    {
        builder.WithGroupName("");
    }

    protected override async Task<Result<Response>> OnRequest(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        await Task.CompletedTask;

        return new Response();
    }
}

public sealed class ExampleConsumer : 
    FeatureSlice<ExampleConsumer.Request, FromServices<Dependency1, Dependency2>>
    .WithConsumer
    .Build<ExampleConsumer>
{
    public record Request();

    protected override ConsumerName ConsumerName => new("ExampleConsumer");

    protected override Task<Result> OnRequest(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        return Result.Success;
    }
}

public sealed class ExampleConsumerWithEndpoint : 
    FeatureSlice<ExampleConsumerWithEndpoint.Request, FromServices<Dependency1, Dependency2>>
    .WithConsumer
    .WithEndpoint
    .Build<ExampleConsumerWithEndpoint>
{
    public record Request();

    protected override ConsumerName ConsumerName => new("ExampleConsumerWithEndpoint");

    protected override Endpoint Endpoint => Map.Get("test", (int age) => 
    {
        return Results.Ok();
    });

    protected override Task<Result> OnRequest(Request request, FromServices<Dependency1, Dependency2> dependencies)
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
        ExampleHandler2.Dispatch handler,
        ExampleConsumerWithEndpoint.Dispatch consumerWithEndpoint)
    {
        publisher.Publish(new ExampleConsumer.Request());
        consumer(new ExampleConsumer.Request());
        handler(new ExampleHandler2.Request());
        consumerWithEndpoint(new ExampleConsumerWithEndpoint.Request());
    }

    public static void Register(IServiceCollection services, IConsumerSetup setup, WebAppExtender hostExtender)
    {
        ExampleConsumer.Register(services, setup);
        ExampleHandler2.Register(services, hostExtender);
        ExampleConsumerWithEndpoint.Register(services, setup, hostExtender);
    }
}