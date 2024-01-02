
namespace FeatureSlice.New;

public record ExampleRequest();
public record ExampleResponse();

public interface IExampleFeatureSlice : IFeatureSlice<ExampleRequest, ExampleResponse>
{
}

public class ExampleFeatureSlice : IExampleFeatureSlice
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
}