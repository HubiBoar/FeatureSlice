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
        .BuildAs<ExampleFeature>,
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
        .AsHandler<ExampleStaticHandler.Request, ExampleStaticHandler.Response, FromServices<Dependency1, Dependency2>>
        .AsEndpoint
        .AsFlag
        .BuildAs<ExampleStaticHandler>,
        IStaticHandler<ExampleStaticHandler.Request, ExampleStaticHandler.Response, FromServices<Dependency1, Dependency2>>,
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


public sealed class ExampleFeatureSelf : 
    FeatureSlice
        .WithHandler<ExampleFeatureSelf.Request, ExampleFeatureSelf.Response, ExampleFeatureSelf.Handler>
        .AsEndpoint
        .AsFlag
        .Build<ExampleFeatureSelf>
{
    public record Request();
    public record Response();

    protected override string FeatureName => "ExampleFeature";

    protected override IEndpoint.Setup Endpoint => IEndpoint.MapGet("test", (int age) => 
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

public sealed class ExampleStaticHandlerSelf : 
    FeatureSlice
        .AsHandler<ExampleStaticHandlerSelf.Request, ExampleStaticHandlerSelf.Response, FromServices<Dependency1, Dependency2>>
        .AsEndpoint
        .AsFlag
        .Build<ExampleStaticHandlerSelf>
{
    public record Request();
    public record Response();

    protected override string FeatureName => "ExampleFeature";

    protected override IEndpoint.Setup Endpoint => IEndpoint.MapGet("test", (int age) => 
    {
        return Results.Ok();
    });

    protected override Task<OneOf<Response, Error>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;
        throw new NotImplementedException();
    }
}

public sealed class ExampleStaticConsumerSelf : 
    FeatureSlice
        .AsConsumer<ExampleStaticConsumerSelf.Request, FromServices<Dependency1, Dependency2>>
        .AsEndpoint
        .AsFlag
        .Build<ExampleStaticConsumerSelf>
{
    public record Request();

    protected override string FeatureName => "ExampleFeature";
    protected override ConsumerName ConsumerName => FeatureName;

    protected override IEndpoint.Setup Endpoint => IEndpoint.MapGet("test", (int age) => 
    {
        return Results.Ok();
    });

    protected override Task<OneOf<Success, Error>> Consume(Request request, FromServices<Dependency1, Dependency2> dependencies)
    {
        var (dep1, dep2) = dependencies;

        throw new NotImplementedException();
    }
}

public class Usage
{
    public static void Use(
        ExampleFeature.Dispatch dispatch1,
        ExampleStaticHandler.Dispatch dispatch2,
        ExampleFeatureSelf.Dispatch dispatch3,
        ExampleStaticHandlerSelf.Dispatch dispatch4,
        ExampleStaticConsumerSelf.Dispatch dispatch5)
    {
        dispatch1.Invoke(new ExampleFeature.Request());
        dispatch2.Invoke(new ExampleStaticHandler.Request());
        dispatch3.Invoke(new ExampleFeatureSelf.Request());
        dispatch4.Invoke(new ExampleStaticHandlerSelf.Request());
        dispatch5.Invoke(new ExampleStaticConsumerSelf.Request());
    }

    public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
    {
        ExampleFeature.Register(services, hostExtender);
        ExampleStaticHandler.Register(services, hostExtender);
        ExampleFeatureSelf.Register(services, hostExtender);
        ExampleStaticHandlerSelf.Register(services, hostExtender);
        ExampleStaticConsumerSelf.Register(services, setupProvider, hostExtender);
    }
}