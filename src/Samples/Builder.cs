using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.Samples.Builder;

public sealed record Dependency1();
public sealed record Dependency2();

public sealed class ExampleHandler2 :
    FeatureSlice2<ExampleHandler2, ExampleHandler2.Request, ExampleHandler2.Response>
{
    public record Request();
    public record Response();

    public override Options Setup => Handle(static async (Request request, Dependency1 dep1, Dependency2 dep2) => 
    {
        await Task.CompletedTask;

        return new Response();
    })
    .AddEndpoint(HttpMethod.Get, "test")
        .FromBody()
        .DefaultResult()
    .WithName("name");
}
public class Usage
{
    public static void Use 
    (
        ExampleHandler2.Dispatch handler
    )
    {
        handler(new ExampleHandler2.Request());
    }

    public static void Register(IServiceCollection services)
    {
        ExampleHandler2.Register(services);
    }
}