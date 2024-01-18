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

public interface IRegistrable
{
    public static abstract void Register(IServiceCollection services);
}

public static class RegistrableExtensions
{
    public static void Register<T>(this IServiceCollection services)
        where T : IRegistrable
    {
        T.Register(services);
    }
}

public static class NotGeneratedClass
{
    public sealed record Disabled();

    public abstract class FeatureSlice<TFeature, TRequest, TResponse> : IRegistrable
        where TFeature : FeatureSlice<TFeature, TRequest, TResponse>
    {
        public abstract string FeatureName { get; }

        public abstract Task<TResponse> Handle(TRequest request);

        public delegate Task<OneOf<TResponse, Disabled>> Dispatch(TRequest request);

        private sealed class Dispatcher
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
                if(await _featureManager.IsEnabledAsync(_feature.FeatureName))
                {
                    return new Disabled();    
                }

                return await _pipelines.RunPipeline(request, _feature.Handle);
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services.AddSingleton<TFeature>();
            services.AddSingleton<Dispatcher>();
            services.AddSingleton<Dispatch>(provider => provider.GetRequiredService<Dispatcher>().Send);
        }
    }

    public interface IMethodPipeline<TRequest, TResponse>
    {
        public delegate TResponse Next(TRequest request);

        public TResponse Handle(TRequest request, Next next);

        public static TResponse RunPipeline(
            TRequest request,
            Func<TRequest, TResponse> featureMethod,
            IReadOnlyList<IMethodPipeline<TRequest, TResponse>> pipelines)
        {
            return RunPipeline(request, featureMethod, 0 , pipelines);
        }

        private static TResponse RunPipeline(
            TRequest request,
            Func<TRequest, TResponse> lastMethod,
            int index,
            IReadOnlyList<IMethodPipeline<TRequest, TResponse>> pipelines)
        {
            if (index < pipelines.Count)
            {
                return pipelines[index].Handle(request, r => RunPipeline(r, lastMethod, index++, pipelines));
            }
            else
            {
                return lastMethod.Invoke(request);
            }
        }
    }

    public static TResponse RunPipeline<TRequest, TResponse>(this IReadOnlyList<IMethodPipeline<TRequest, TResponse>> pipelines, TRequest request, Func<TRequest, TResponse> featureMethod)
    {
        return IMethodPipeline<TRequest, TResponse>.RunPipeline(request, featureMethod, pipelines);
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

    public sealed class ExampleFeature : FeatureSlice<ExampleFeature, ExampleFeature.Request, ExampleFeature.Response>
    {
        public sealed record Request();
        public sealed record Response();
        public override string FeatureName => "ExampleFeature";

        public ExampleFeature(Dependency dependency)
        {
        }

        public override Task<Response> Handle(Request request)
        {
            throw new NotImplementedException();
        }
    }
}



public class Dependency
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<Dependency>();
        //NotGenerated.IExampleFeature.Register<NotGenerated.ExampleFeature>();
        Generated.ExampleFeature.Register(services);
        //NotGeneratedClass.ExampleFeature.Register(services);
        services.Register<NotGeneratedClass.ExampleFeature>();
    }

    public async Task Run(
        NotGenerated.IExampleFeature.IDispatcher notDispatcher,
        Generated.ExampleFeature.Dispatch dispatch,
        NotGeneratedClass.ExampleFeature.Dispatch notGeneratedDispatch)
    {
        await notDispatcher.Send(new NotGenerated.IExampleFeature.Request());
        await dispatch(new Generated.ExampleFeature.Request());
        await notGeneratedDispatch(new NotGeneratedClass.ExampleFeature.Request());
    }
}


//Auto generate Dependencies code
public sealed partial class DependencyChecker :
    IDependencyProvide<Dependency>,
    IDependencyProvide<IFeatureManager>,
    IDependencyProvide<Generated.ExampleFeature>,
    IDependencyProvide<NotGeneratedClass.ExampleFeature.Dispatch>,
    IDependencyProvide<Generated.ExampleFeature.Dispatch>
{
}

public sealed partial class DependencyChecker :
    IDependencyRequire<Dependency, DependencyChecker>,
    IDependencyRequire<Generated.ExampleFeature, DependencyChecker>,
    IDependencyRequire<IFeatureManager, DependencyChecker>,
    IDependencyRequire<NotGeneratedClass.ExampleFeature.Dispatch, DependencyChecker>,
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