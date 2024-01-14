using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using FeatureSlice.New.Generation;

namespace Samples;

internal sealed class ExampleMethod : IExampleMethod
{
    public IExampleMethod.Response Handle(IExampleMethod.Request request)
    {
        return new IExampleMethod.Response();
    }
}

public interface IExampleMethod2 : IMethod<IExampleMethod2.Request, IExampleMethod2.Response>
{
    public sealed record Request();
    public sealed record Response();
}

internal sealed class ExampleMethod2 : IExampleMethod2
{
    public IExampleMethod2.Response Handle(IExampleMethod2.Request request)
    {
        return new IExampleMethod2.Response();
    }
}


internal class ExampleFeatureSlice : IExampleFeatureSlice
{
    public string FeatureName => "ExampleFeatureSlice";

    public Task<IExampleFeatureSlice.Response> Handle(IExampleFeatureSlice.Request request)
    {
        return Task.FromResult(new IExampleFeatureSlice.Response());
    }
}


internal sealed class ExampleMessageConsumer : IExampleMessageConsumer
{
    public string FeatureName => "ExampleMessageConsumer";

    public Task<OneOf<Success, Retry, Error>> Handle(IExampleMessageConsumer.Message request)
    {
        return Task.FromResult(OneOf<Success, Retry, Error>.FromT0(new Success()));
    }
}
public sealed class Dependncy
{
    public static void Register(IServiceCollection services)
    {
        services.AddFeature<IExampleFeatureSlice, ExampleFeatureSlice>();
        services.AddFeature<IExampleMethod, ExampleMethod>();
        services.AddFeature<IExampleMethod2, ExampleMethod2>();
        services.AddFeature<IExampleMessageConsumer, ExampleMessageConsumer>();
    }

    private readonly IDispatcher<IExampleMethod> _method;
    private readonly IDispatcher<IExampleMethod2> _method2;
    private readonly IDispatcher<IExampleFeatureSlice> _featureSlice;
    private readonly IDispatcher<IExampleMessageConsumer> _consumer;

    public Dependncy(IDispatcher<IExampleMethod> method, IDispatcher<IExampleFeatureSlice> featureSlice, IDispatcher<IExampleMessageConsumer> consumer, IDispatcher<IExampleMethod2> method2)
    {
        _method = method;
        _featureSlice = featureSlice;
        _consumer = consumer;
        _method2 = method2;
    }

    private async Task Run()
    {
        var methodResult = _method.Send(new IExampleMethod.Request());
        var method2Result = _method2.Send(new IExampleMethod2.Request());
        var featureResult = await _featureSlice.Send(new IExampleFeatureSlice.Request());
        var consumerResult = await _consumer.Send(new IExampleMessageConsumer.Message());
    }
}