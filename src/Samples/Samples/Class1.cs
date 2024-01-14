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

public record T1Example()
{
    public Task Handle()
    {
    }
}
public record T2Example()
{
    public Task Handle()
    {
    }
}
public record T3Example()
{
    public Task Handle()
    {
    }
}

public interface IPart<T>
{
    
}

public interface IOneOf<T1, T2, T3>
{
}

public sealed record Is_T1Example(T1Example Value) : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T3Example, T2Example, T1Example>
{
    public static explicit operator Is_T1Example(T1Example value)
    {
        return new Is_T1Example(value);
    }
}

public sealed record Is_T2Example(T2Example Value) : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T3Example, T2Example, T1Example>
{
}

public sealed record Is_T3Example(T3Example Value) : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T3Example, T2Example, T1Example>
{
}

public static class OneOfExtensions
{
    public static IOneOf<T1Example, T2Example, T3Example> ToReturn(T1Example t1)
    {
        return (Is_T1Example)t1;
    }
}

public sealed class Dependncy
{
    public async Task Handle(IOneOf<T1Example, T2Example, T3Example> oneOf)
    {
        var task = oneOf switch
        {
            Is_T1Example t => t.Value.Handle(),
            Is_T2Example t => t.Value.Handle(),
            Is_T3Example t => t.Value.Handle(),
            _ => Task.CompletedTask
        };

        await task;
    }

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