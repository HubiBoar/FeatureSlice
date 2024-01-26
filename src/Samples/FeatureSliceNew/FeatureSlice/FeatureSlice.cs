using FeatureSlice;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace Samples;

internal static partial class ExampleFeature
{
    public sealed record Request();
    public sealed record Response();

    public sealed partial class FeatureSlice1 : IFeatureSlice<Request, Response>, IRegistrable
    {
        public Task<Response> Handle(Request request)
        {
            return Task.FromResult(new Response());
        }
    }

    public sealed partial class FeatureSlice2 : IFeatureSlice<Request, Response>.WithToggle, IRegistrable
    {
        public static string FeatureName => "FeatureSlice2";

        public Task<Response> Handle(Request request)
        {
            return Task.FromResult(new Response());
        }
    }
}

internal static partial class ExampleFeature
{
    public static void Register(IServiceCollection services)
    {
        services.Register<FeatureSlice1>();
        services.Register<FeatureSlice2>();
    }

    public static async Task Run(FeatureSlice1.Dispatch slice1, FeatureSlice2.Dispatch slice2)
    {
        var result1 = await slice1(new Request());
        var result2 = await slice2(new Request());

        result2.Switch(
            resposnse => {},
            disabled => {});
    }
}

//Autogeneratoed
internal static partial class ExampleFeature
{
    public sealed partial class FeatureSlice1
    {
        public delegate Task<Response> Dispatch(Request request);

        public static void Register(IServiceCollection services)
        {
            IFeatureSlice<Request, Response>.Runner<FeatureSlice1>.Register<Dispatch>(
                services,
                provider => request => IFeatureSlice<Request, Response>.Runner<FeatureSlice1>.Factory(provider).Invoke(request));
        }
    }

    public sealed partial class FeatureSlice2
    {
        public delegate Task<OneOf<Response, Disabled>> Dispatch(Request request);

        public static void Register(IServiceCollection services)
        {
            IFeatureSlice<Request, Response>.WithToggle.Runner<FeatureSlice2>.Register<Dispatch>(
                services,
                provider => request => IFeatureSlice<Request, Response>.WithToggle.Runner<FeatureSlice2>.Factory(provider).Invoke(request));
        }
    }
}