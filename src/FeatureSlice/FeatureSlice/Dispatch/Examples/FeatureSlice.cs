using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Dispatch.Examples.Slice;

public partial class Example : IRegistrable.IWeb
{
    public record Request();

    public record Response();

    public sealed class Feature : FeatureSlice<Feature, Request, Response>, IFeatureName
    {
        public static string FeatureName => "ExampleFeature";

        public override Task<Response> Handle(Request request)
        {
            return Task.FromResult(new Response());
        }
    }

    internal sealed class Endpoint : IEndpoint
    {
        public static EndpointSetup Setup => EndpointSetup
            .MapGet("/test", TestMethod)
            .WithName("name");

        private static IResult TestMethod(string param, [FromServices] Feature feature)
        {
            feature.Handle(new Request());
            return Results.Ok();
        }
    }

    public static void Register(IApplicationSetup<WebApplication> setup)
    {
        setup.Register<Feature>();
        setup.Map<Endpoint>();
    }
}

public class ExampleUsage
{
    private readonly Example.Feature.Dispatcher _dispatcher;

    public ExampleUsage(Example.Feature.Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public static void Register(IApplicationSetup<WebApplication> setup)
    {
        setup.Register<Example>();
    }

    public async Task Invoke()
    {
        await _dispatcher(new Example.Request());
    }
}