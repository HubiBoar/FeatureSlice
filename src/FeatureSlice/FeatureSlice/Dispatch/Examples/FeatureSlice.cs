using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Dispatch.Examples;

public partial class Example : IRegistrable
{
    public record Request();

    public record Response();

    public sealed class Feature : FeatureSlice<Feature, Request, Response>, IFeatureName
    {
        public static string FeatureName => "Feature";

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

    public static void Register(IServiceCollection services)
    {
        services.Register<Feature>();
        services.Map<Endpoint>();
    }
}

public class ExampleUsage
{
    private readonly Example.Feature.IDispatcher _dispatcher;

    public ExampleUsage(Example.Feature.IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public static void Register(IServiceCollection services)
    {
        services.Register<Example>();
    }

    public async Task Invoke()
    {
        await _dispatcher.Send(new Example.Request());
    }
}