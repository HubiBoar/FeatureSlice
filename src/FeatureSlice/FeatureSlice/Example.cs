using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public class ExampleRequest
{
}

public class ExampleResponse
{
}

public interface IFeatureExample : IFeatureSlice<ExampleRequest, ExampleResponse>
{
    static string IFeatureToggle.FeatureName => "FeatureExample";
}

internal sealed class FeatureExample : IFeatureExample
{
    public IDispatcher<ExampleRequest, ExampleResponse> Dispatcher { get; }
    
    public FeatureExample(IDispatcher<ExampleRequest, ExampleResponse> dispatcher)
    {
        Dispatcher = dispatcher;
    }

    public Task<ExampleResponse> Handle(ExampleRequest request)
    {
        return Task.FromResult(new ExampleResponse());
    }
}

public class RegistrationExample
{
    public void Register(IServiceCollection collection)
    {
        collection.AddFeature<IFeatureExample, FeatureExample>();
    }

    public void Send(IFeatureExample example)
    {
        var result = example.Send(new ExampleRequest());
    }
}