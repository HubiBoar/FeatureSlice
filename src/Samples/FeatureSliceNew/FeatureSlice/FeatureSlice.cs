using FeatureSlice;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace Samples;

internal sealed partial class ExampleFeatureSlice : IRegistrable<HostExtender<WebApplication>>
{
    public sealed record Request();
    public sealed record Response();

    public sealed partial class Handler : Feature.IHandler<Request, Response>
    {
        public static string Name => "Handler";

        public Task<OneOf<Response, Error>> Handle(Request request)
        {
            throw new Exception();
        }
    }

    public sealed partial class Endpoint : Feature.IEndpoint
    {
        public static Feature.EndpointSetup Setup => Feature.EndpointSetup
            .MapGet("/test", TestMethod)
            .WithName("name");

        private static IResult TestMethod(string param, [FromServices] Handler feature)
        {
            feature.Handle(new Request());
            return Results.Ok();
        }
    }

    public static void Register(IServiceCollection services, HostExtender<WebApplication> host)
    {
        host.Map<Endpoint>();
        Handler.Register(services);
    }
}

internal static class ExampleFeatureSliceRunner
{
    public static void Register(IServiceCollection services, HostExtender<WebApplication> host)
    {
        ExampleFeatureSlice.Register(services, host);
    }

    public static async Task Run(ExampleFeatureSlice.Handler.Dispatch slice1, Publisher<ExampleFeatureSlice.Request>.Dispatch dispatch)
    {
        var result1 = await slice1(new ExampleFeatureSlice.Request());
        var result3 = await dispatch(new ExampleFeatureSlice.Request());
    }
}

//Autogeneratoed
internal sealed partial class ExampleFeatureSlice
{
    public sealed partial class Handler
    {
        public delegate Task<OneOf<Response, Disabled, Error>> Dispatch(Request request);

        public static void Register(IServiceCollection services)
        {
            Feature.IHandler<Request, Response>.Setup<Handler>.Register<Dispatch>(
                services,
                provider => request => Feature.IHandler<Request, Response>.Setup<Handler>.DispatchFactory(provider).Invoke(request));
        }
    }
}