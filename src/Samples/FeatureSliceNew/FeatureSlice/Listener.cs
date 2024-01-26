using FeatureSlice;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace Samples;

internal static partial class ExampleListener
{
    public sealed record Request();

    private sealed partial class Listener1 : IListener<Request>, IRegistrable
    {
        public Task Handle(Request request)
        {
            return Task.CompletedTask;
        }
    }

    private sealed partial class Listener2 : IListener<Request>.WithToggle, IRegistrable
    {
        public static string FeatureName => "Listener2";

        public Task Handle(Request request)
        {
            return Task.CompletedTask;
        }
    }
}

internal static partial class ExampleListener
{
    public static void Register(IServiceCollection services)
    {
        services.Register<Listener1>();
        services.Register<Listener2>();
    }

    public static Task Run(IListener<Request>.Dispatch listener)
    {
        return listener(new Request());
    }
}

//Autogeneratoed
internal static partial class ExampleListener
{
    private sealed partial class Listener1
    {
        public static void Register(IServiceCollection services)
        {
            IListener<Request>.Runner<Listener2>.Register(services);
        }
    }

    private sealed partial class Listener2
    {
        public static void Register(IServiceCollection services)
        {
            IListener<Request>.Runner<Listener2>.Register(services);
        }

        public Task<bool> IsEnabled(IFeatureManager featureManager)
        {
            return featureManager.IsEnabledAsync(FeatureName);
        }
    }
}