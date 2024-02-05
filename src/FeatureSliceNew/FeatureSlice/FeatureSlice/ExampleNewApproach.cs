using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;


public sealed partial class Example
{
    public record Request();

    public record Response();

    public partial class ExampleFeature : Feature.IHandler<Request, Response>
    {
        public static string Name => "lol";

        public Task<OneOf<Response, Error>> Handle(Request request)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ExampleConsumer : Feature.IHandler<Request, Success>
    {
        public static string Name => "lol";

        public Task<OneOf<Success, Error>> Handle(Request request)
        {
            throw new NotImplementedException();
        }
    }


    public static void Register(IDispatcherModule dispatcherModule, IMessagingModule module)
    {
        dispatcherModule.Register<ExampleFeature>();
        module.Register<Request, ExampleConsumer>();
    }

    public static void Use(ExampleFeature.Dispatch dispatcher)
    {
        dispatcher(new Request());
    }
}

//AutoGenerated
public sealed partial class Example
{
    public partial class ExampleFeature : Feature.IHandler<Request, Response>.IRegistrable<ExampleFeature, ExampleFeature.Dispatch>
    {
        public delegate Task<OneOf<Response, Disabled, Error>> Dispatch(Request request);

        public static Dispatch Convert(IServiceProvider provider, Feature.Dispatch<Request, Response, ExampleFeature> dispatcher)
        {
            return request => dispatcher(request, provider);
        }
    }

    public partial class ExampleConsumer : Feature.IHandler<Request, Success>.IRegistrable<ExampleConsumer, ExampleConsumer.Dispatch>
    {
        public delegate Task<OneOf<Success, Disabled, Error>> Dispatch(Request request);

        public static Dispatch Convert(IServiceProvider provider, Feature.Dispatch<Request, Success, ExampleConsumer> dispatcher)
        {
            return request => dispatcher(request, provider);
        }
    }
}







