using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice2.NewDispatcher;

public static class NotGenerated
{
    public sealed record Disabled();

    //Generators would be helpfull for Pipelines
    public interface IFeatureSlice<TSelf, TRequest, TResponse>
        where TSelf : class, IFeatureSlice<TSelf, TRequest, TResponse>
    {
        public string FeatureName { get; }

        public Task<TResponse> Handle(TRequest request);

        public interface IDispatcher
        {
            public Task<OneOf<TResponse, Disabled>> Send(TRequest request);
        }

        internal class Dispatcher : IDispatcher
        {
            private readonly TSelf _feature;
            private readonly IFeatureManager _featureManager;
            private readonly string _featureName;

            public Dispatcher(TSelf feature, IFeatureManager featureManager)
            {
                _feature = feature;
                _featureManager = featureManager;
                _featureName = feature.FeatureName;
            }

            public async Task<OneOf<TResponse, Disabled>> Send(TRequest request)
            {
                if(await _featureManager.IsEnabledAsync(_featureName))
                {
                    return new Disabled();    
                }

                return await _feature.Handle(request);
            }
        }

        public static void Register<TImplementation>(IServiceCollection services)
            where TImplementation : class, TSelf
        {
            services.AddFeatureManagement();
            services.AddSingleton<TSelf, TImplementation>();
        }
    }

    public interface IExampleFeature : IFeatureSlice<IExampleFeature, IExampleFeature.Request, IExampleFeature.Response>
    {
        public sealed record Request();
        public sealed record Response();

        internal sealed class Feature : IExampleFeature
        {
            public string FeatureName => "ExampleFeature";

            public Task<Response> Handle(Request request)
            {
                throw new NotImplementedException();
            }
        }
    }
}

public static class Generated
{
    public sealed record Disabled();

    public interface IFeatureSlice<TRequest, TResponse>
    {
        public static abstract string FeatureName { get; }

        public Task<TResponse> Handle(TRequest request);

        public interface IDispatcher<TFeature>
            where TFeature : class, IFeatureSlice<TRequest, TResponse>
        {
            public Task<OneOf<TResponse, Disabled>> Send(TRequest request);
        }

        internal sealed class Dispatcher<TFeature> : IDispatcher<TFeature>
            where TFeature : class, IFeatureSlice<TRequest, TResponse>
        {
            private readonly TFeature _feature;
            private readonly IFeatureManager _featureManager;
            private readonly IReadOnlyList<IMethodPipeline<TRequest, Task<TResponse>>> _pipelines;

            public Dispatcher(TFeature feature, IFeatureManager featureManager, IEnumerable<IMethodPipeline<TRequest, Task<TResponse>>> pipelines)
            {
                _feature = feature;
                _featureManager = featureManager;
                _pipelines = pipelines.ToList();
            }

            public async Task<OneOf<TResponse, Disabled>> Send(TRequest request)
            {
                if(await _featureManager.IsEnabledAsync(TFeature.FeatureName))
                {
                    return new Disabled();    
                }

                return await IMethodPipeline<TRequest, Task<TResponse>>.RunPipeline(request, _feature.Handle, 0, _pipelines);
            }
        }

        public static void Register<TFeature>(IServiceCollection services)
            where TFeature : class, IFeatureSlice<TRequest, TResponse>
        {
            services.AddFeatureManagement();
            services.AddSingleton<TFeature>();
            services.AddSingleton<IDispatcher<TFeature>, Dispatcher<TFeature>>();
        }
    }

    public interface IMethodPipeline<TRequest, TResponse>
    {
        public delegate TResponse Next(TRequest request);

        public TResponse Handle(TRequest request, Next next);

        public static TResponse RunPipeline(
            TRequest request,
            Func<TRequest, TResponse> featureMethod,
            int index,
            IReadOnlyList<IMethodPipeline<TRequest, TResponse>> pipelines)
        {
            if (index < pipelines.Count)
            {
                return pipelines[index].Handle(request, r => RunPipeline(r, featureMethod, index++, pipelines));
            }
            else
            {
                return featureMethod.Invoke(request);
            }
        }
    }

    public sealed class ExamplePipeline<TRequest, TResponse> : IMethodPipeline<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public TResponse Handle(TRequest request, IMethodPipeline<TRequest, TResponse>.Next next)
        {
            return next(request);
        }
    }

    public sealed partial class ExampleFeature : IFeatureSlice<ExampleFeature.Request, ExampleFeature.Response>
    {
        public sealed record Request();
        public sealed record Response();
        public static string FeatureName => "ExampleFeature";

        public ExampleFeature(Dependency dependency)
        {
        }

        public Task<Response> Handle(Request request)
        {
            throw new NotImplementedException();
        }
    }

    //AutoGenerated
    public partial class ExampleFeature
    {
        public delegate Task<OneOf<Response, Disabled>> Dispatch(Request request);

        public static void Register(IServiceCollection services)
        {
            IFeatureSlice<Request, Response>.Register<ExampleFeature>(services);
            services.AddSingleton<Dispatch>(provider => provider.GetRequiredService<IFeatureSlice<Request, Response>.IDispatcher<ExampleFeature>>().Send);
        }
    }
}


public class Dependency
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<Dependency>();
        Generated.ExampleFeature.Register(services);
    }

    public async Task Run(
        NotGenerated.IExampleFeature.IDispatcher notDispatcher,
        Generated.ExampleFeature.Dispatch dispatch)
    {
        await notDispatcher.Send(new NotGenerated.IExampleFeature.Request());
        await dispatch(new Generated.ExampleFeature.Request());
    }
}


//Auto generate Dependencies code
public sealed partial class DependencyChecker :
    IDependencyProvide<Dependency>,
    IDependencyProvide<IFeatureManager>,
    IDependencyProvide<Generated.ExampleFeature>,
    IDependencyProvide<Generated.ExampleFeature.IDispatcher>,
    IDependencyProvide<Generated.ExampleFeature.Dispatcher>,
    IDependencyProvide<Generated.ExampleFeature.Dispatch>
{
}

public sealed partial class DependencyChecker :
    IDependencyRequire<Dependency, DependencyChecker>,
    IDependencyRequire<Generated.ExampleFeature, DependencyChecker>,
    IDependencyRequire<IFeatureManager, DependencyChecker>,
    IDependencyRequire<Generated.ExampleFeature.Dispatcher, DependencyChecker>,
    IDependencyRequire<Generated.ExampleFeature.Dispatch, DependencyChecker>
{
}

public interface IDependencyProvide<T>
{
}

public interface IDependencyRequire<T, TProvider>
    where TProvider : IDependencyProvide<T>
{
}

public sealed partial class DependencyChecker
{
}