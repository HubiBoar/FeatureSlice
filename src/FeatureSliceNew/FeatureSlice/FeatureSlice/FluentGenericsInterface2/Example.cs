using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Http;

namespace FeatureSlice.FluentGenerics.Interfaces2;

public sealed class ExampleFeature : 
    FeatureSlice
        .AsFlag
        .WithEndpoint<ExampleFeature.Endpoint>
        .WithHandler<ExampleFeature.Request, ExampleFeature.Response, ExampleFeature.Handler>
        .Build<ExampleFeature>,
        IFeatureFlag
{
    public record Request();
    public record Response();

    public static string FeatureName => "ExampleFeature";

    public class Handler : IHandler<Request, Response>
    {
        public Task<OneOf<Response, Error>> Handle(Request response)
        {
            throw new NotImplementedException();
        }
    }

    public class Endpoint : EndpointHelper, IEndpoint
    {
        public static EndpointInfo Info => MapGet("test", (int age) => 
        {
            return Results.Ok();
        });
    }
}

public sealed class ExampleAsEndpoint : 
    FeatureSlice
        .AsFlag
        .AsEndpoint
        .WithHandler<ExampleAsEndpoint.Request, ExampleAsEndpoint.Response, ExampleAsEndpoint.Handler>
        .Build<ExampleAsEndpoint>,
        IEndpoint,
        IFeatureFlag
{
    public record Request();
    public record Response();

    public static string FeatureName => "ExampleFeature";

    public static EndpointInfo Info => IEndpoint.MapGet("test", (int age) => 
    {
        return Results.Ok();
    });

    public class Handler : IHandler<Request, Response>
    {
        public Task<OneOf<Response, Error>> Handle(Request response)
        {
            throw new NotImplementedException();
        }
    }
}

public sealed class ExampleAsEndpointAndHandler : 
    FeatureSlice
        .AsFlag
        .AsEndpoint
        .AsHandler<ExampleAsEndpointAndHandler.Request, ExampleAsEndpointAndHandler.Response>
        .Build<ExampleAsEndpointAndHandler>,
        IEndpoint,
        IFeatureFlag,
        IHandler<ExampleAsEndpointAndHandler.Request, ExampleAsEndpointAndHandler.Response>
{
    public record Request();
    public record Response();

    public static string FeatureName => "ExampleFeature";

    public static EndpointInfo Info => IEndpoint.MapGet("test", (int age) => 
    {
        return Results.Ok();
    });

    public Task<OneOf<Response, Error>> Handle(Request response)
    {
        throw new NotImplementedException();
    }
}


public sealed class ExampleConsumer : 
    FeatureSlice
        .AsFlag
        .WithEndpoint<ExampleConsumer.Endpoint>
        .WithConsumer<ExampleConsumer.Request, ExampleConsumer.Consumer>
        .Build<ExampleConsumer>,
        IFeatureFlag
{
    public record Request();

    public static string FeatureName => "ExampleConsumer";

    public class Consumer : IConsumer<Request>
    {
        public Task<OneOf<Success, Error>> Handle(Request request)
        {
            throw new NotImplementedException();
        }
    }

    public class Endpoint : EndpointHelper, IEndpoint
    {
        public static EndpointInfo Info => MapGet("test", (int age) => 
        {
            return Results.Ok();
        });
    }
}

public class Usage
{
    public static void Use(ExampleFeature.Dispatch dispatch)
    {
        dispatch.Invoke(new ExampleFeature.Request());
    }

    public static void Use(ExampleConsumer.Dispatch dispatch)
    {
        dispatch.Invoke(new ExampleConsumer.Request());
    }

    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
    {
        ExampleFeature.Register(services, hostExtender);
    }
}

