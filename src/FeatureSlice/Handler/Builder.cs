using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public static class FeatureSliceExtensions
{
    public static FeatureSlice2<TSelf, TRequest, TResponse>.ISetup AddEndpoint<TSelf, TRequest, TResponse>
    (
        this FeatureSlice2<TSelf, TRequest, TResponse>.ISetup setup
    )
        where TSelf : FeatureSlice2<TSelf, TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        return setup;
    }

    public static FeatureSlice2<TSelf, TRequest, TResponse>.IBuilt Build<TSelf, TRequest, TResponse>
    (
        this FeatureSlice2<TSelf, TRequest, TResponse>.ISetup setup,
        ServiceLifetime serviceLifetime
    )
        where TSelf : FeatureSlice2<TSelf, TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {

        return setup;
    }
}

public abstract class FeatureSlice2<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
}

public abstract class FeatureSlice2<TSelf, TRequest, TResponse>
    where TSelf : FeatureSlice2<TSelf, TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    public sealed class Options
    { 
        public ServiceLifetime ServiceLifetime { get; set; }
        public Func<IServiceProvider, Dispatch, Dispatch> Dispatcher { get; set; }

        private readonly List<Action<IServiceCollection>> _extensions;

        internal Options()
        {
            _extensions = new ();
            Dispatcher = (_, dispatch) => dispatch;
        }

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        internal void Register
        (
            IServiceCollection services
        )
        {
            foreach(var extension in _extensions)
            {
                extension(services);
            }

            const string key = "Handler";

            services.AddKeyed<TSelf>(ServiceLifetime, key);
            services.Add(ServiceLifetime, GetDispatch);

            Dispatch GetDispatch(IServiceProvider provider)
            {
                var self = provider.GetRequiredKeyedService<TSelf>(key);

                var dispatch = Dispatcher
                (
                    provider,
                    self.Handle
                );

                return request => dispatch(request);
            }
        }
    }

    public delegate Task<Result<TResponse>> Dispatch(TRequest request);

    public abstract void Setup(Options options);

    protected abstract Task<Result<TResponse>> Handle(TRequest request); 

    public static void Register
    (
        IServiceCollection services,
        ServiceFactory<IHandlerSetup> setupFactory
    )
    {

    }
}


public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, Result<TResponse>, TResponse, TDependencies>
        where TSelf : Build<TSelf>, new()
    {
        public static void Register(IServiceCollection services)
        {
            RegisterHandler(services);
        }
 
        public static void Register
        (
            IServiceCollection services,
            ServiceFactory<IHandlerSetup> setupFactory
        )
        {
            RegisterHandler(services, setupFactory);
        }
    }
}

public static partial class FeatureSlice<TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, Result, Success, TDependencies>
        where TSelf : Build<TSelf>, new()
    {
        public static void Register(IServiceCollection services)
        {
            RegisterHandler(services);
        }
 
        public static void Register
        (
            IServiceCollection services,
            ServiceFactory<IHandlerSetup> setupFactory
        )
        {
            RegisterHandler(services, setupFactory);
        }
    }
}