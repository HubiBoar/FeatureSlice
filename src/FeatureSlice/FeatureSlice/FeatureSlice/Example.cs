using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Http;

namespace FeatureSlice;

public sealed class ExampleFeature : 
    FeatureSlice
        .WithHandler<ExampleFeature.Request, ExampleFeature.Response, ExampleFeature.Handler>
        .AsEndpoint
        .AsFlag
        .Build<ExampleFeature>,
        IEndpoint,
        IFeatureFlag
{
    public record Request();
    public record Response();

    public static string FeatureName => "ExampleFeature";

    public static IEndpoint.Setup Endpoint => IEndpoint.MapGet("test", (int age) => 
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

public sealed record Dependency1();
public sealed record Dependency2();

public sealed class ExampleStaticHandler : 
    FeatureSlice
        .AsHandler<ExampleStaticHandler.Request, ExampleStaticHandler.Response>
        .AsEndpoint
        .AsFlag
        .Build<ExampleStaticHandler>,
        IStaticHandler<ExampleStaticHandler, ExampleStaticHandler.Request, ExampleStaticHandler.Response, FromServices<Dependency1, Dependency2>>,
        IEndpoint,
        IFeatureFlag
{
    public record Request();
    public record Response();

    public static string FeatureName => "ExampleFeature";

    public static IEndpoint.Setup Endpoint => IEndpoint.MapGet("test", (int age) => 
    {
        return Results.Ok();
    });

    public static Task<OneOf<Response, Error>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;
        throw new NotImplementedException();
    }
}

public class Usage
{
    public static void Use(ExampleFeature.Dispatch dispatch)
    {
        dispatch.Invoke(new ExampleFeature.Request());
    }

    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
    {
        ExampleFeature.Register(services, hostExtender);
    }
}