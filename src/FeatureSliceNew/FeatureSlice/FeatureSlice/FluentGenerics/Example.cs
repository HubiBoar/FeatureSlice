using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Http;

namespace FeatureSlice.FluentGenerics;

public sealed class ExampleFeature : 
    FeatureSlice<ExampleFeature>
        .WithFlag<ExampleFeature>
        .WithEndpoint<ExampleFeature.Endpoint>
        .WithHandler<ExampleFeature.Request, ExampleFeature.Response, ExampleFeature.Handler>,
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
        public static EndpointInfo Map => MapGet("test", (int age) => 
        {
            return Results.Ok();
        });
    }
}


public sealed class ExampleConsumer : 
    FeatureSlice<ExampleConsumer>
        .WithFlag<ExampleConsumer>
        .WithEndpoint<ExampleConsumer.Endpoint>
        .WithConsumer<ExampleConsumer.Request, ExampleConsumer.Consumer>,
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
        public static EndpointInfo Map => MapGet("test", (int age) => 
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

