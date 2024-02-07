using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Http;

namespace FeatureSlice.FluentGenerics.Interfaces;

public sealed partial class ExampleEndpoint : 
    FeatureSlice
        .WithFlag
        .AsEndpoint
{
    public static string FeatureName => "ExampleFeature";

    public static EndpointInfo Map => IEndpoint.Get("test", (int age) => 
    {
        return Results.Ok();
    });
}

public sealed partial class ExampleHandler : 
    FeatureSlice
        .WithFlag
        .AsEndpoint
{
    public static string FeatureName => "ExampleHandler";

    public static EndpointInfo Map => IEndpoint.Get("test", (int age) => 
    {
        return Results.Ok();
    });
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

//Auto Generated
public sealed partial class ExampleEndpoint
{
    public static void Register(HostExtender<WebApplication> hostExtender)
    {
        EndpointFeatureSlice.AsFlag.RegisterInternal<ExampleEndpoint>(hostExtender);
    }
}


public sealed partial class ExampleHandler
{
    public static void Register(HostExtender<WebApplication> hostExtender)
    {
        EndpointFeatureSlice.AsFlag.RegisterInternal<ExampleEndpoint>(hostExtender);
    }
}