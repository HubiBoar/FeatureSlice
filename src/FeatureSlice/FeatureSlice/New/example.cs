using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.New;

public sealed record ExampleRequest();
public sealed record ExampleResponse();

public interface IExampleFeatureSlice : IFeatureSlice<ExampleRequest, ExampleResponse>
{
}

internal sealed class ExampleFeatureSlice : IExampleFeatureSlice
{
    public IFeatureDispatcher? Dispatcher { get; set; }

    public Task<ExampleResponse> Handle(ExampleRequest input)
    {
        throw new NotImplementedException();
    }

    private Task<ExampleResponse> Run(IExampleFeatureSlice slice)
    {
        return slice.Send(new ExampleRequest());
    }

    public void Register(IServiceCollection collection)
    {
        IExampleFeatureSlice.Register<IExampleFeatureSlice, ExampleFeatureSlice>(collection);
    }
}