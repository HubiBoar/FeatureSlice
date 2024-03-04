using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Momolith.Modules;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public sealed class ExampleHandler
{
    public delegate Task<OneOf<Success, Disabled, Error>> DispatchConsume(Request request);
    public delegate Task<OneOf<Response, Disabled, Error>> DispatchWithFlag(Request request);
    public delegate Task<OneOf<Response, Error>> Dispatch(Request request);
    
    public sealed record Request();
    public sealed record Response();

    public static void Register(IServiceCollection services, IHostExtender<WebApplication> extender)
    {
        services.FeatureSlice()
            .WithEndpoint(
                extender,
                e => e.MapGet("test", (int a) => Results.Ok()))
            .WithHandler<Dispatch, Request, Response>(
                provider => request => Handle(request, provider.From()),
                handler => handler.Invoke);
    }

    public static void RegisterWithFlag(IServiceCollection services, IHostExtender<WebApplication> extender)
    {
        services.FeatureSlice()
            .WithFlag("Flag")
            .WithEndpoint(
                extender,
                IEndpoint.MapGet("test", (int a) => Results.Ok()))
            .WithHandler<DispatchWithFlag, Request, Response>(
                (request, deps) => Handle(request, deps),
                handler => handler.Invoke);
    }

    public static void RegisterConsumer(IServiceCollection services, IHostExtender<WebApplication> extender, Messaging.ISetup setup)
    {
        services.FeatureSlice()
            .WithEndpoint(
                extender,
                e => e.MapGet("test", (int a) => Results.Ok()))
            .WithConsumer<DispatchConsume, Request, FromServices<Request, Request>>(
                setup,
                new ConsumerName("consume-test"),
                Consume,
                handler => handler.Invoke);
    }

    private static async Task<OneOf<Success, Error>> Consume(Request request, FromServices<Request, Request> dependencies)
    {
        return new Success();
    }

    private static async Task<OneOf<Response, Error>> Handle(Request request, FromServices<Request, Request> dependencies)
    {
        return new Response();
    }
}