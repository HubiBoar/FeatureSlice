using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.Dispatch.Examples.Consumer;

public partial class Example : IRegistrable
{
    public record Message() : IMessage
    {
        public static string Name => "ExampleRequest";
    }

    public sealed class Consumer : MessageConsumer<Consumer, Message>, IFeatureName
    {
        public static string FeatureName => "ExampleConsumer";

        public override Task<OneOf<Success, Retry, Error>> Handle(MessageContext<Message> context)
        {
            return Task.FromResult(OneOf<Success, Retry, Error>.FromT0(new Success()));
        }
    }

    internal sealed class Endpoint : IEndpoint
    {
        public static EndpointSetup Setup => EndpointSetup
            .MapGet("/test", TestMethod)
            .WithName("name");

        private static IResult TestMethod(string param, [FromServices] Consumer consumer)
        {
            consumer.HandleInMemory(new Message());
            return Results.Ok();
        }
    }

    public static void Register(IServiceCollection services)
    {
        services.Register<Consumer>();
        services.Map<Endpoint>();
    }
}

public class ExampleUsage
{
    private readonly Example.Consumer.IDispatcher _dispatcher;

    public ExampleUsage(Example.Consumer.IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public static void Register(IServiceCollection services)
    {
        services.Register<Example>();
    }

    public async Task Invoke()
    {
        await _dispatcher.Send(new Example.Message());
    }
}