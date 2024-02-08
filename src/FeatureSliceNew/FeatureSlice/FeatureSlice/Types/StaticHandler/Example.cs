using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static class StaticHandlerExample
{
    public sealed record Dependency1();
    public sealed record Dependency2();

    public class Handler : IStaticHandler<Handler.Request, Handler.Response, FromServices<Dependency1, Dependency2>>
    {
        public sealed record Request();

        public sealed record Response();

        public static Task<OneOf<Response, Error>> Handle(Request request, FromServices<Dependency1, Dependency2> dependencies)
        {
            var (dependency1, dependency2) = dependencies;

            throw new Exception();
        }
    }
}