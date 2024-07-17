using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public delegate Task<TResult> Dispatch<TRequest, TResult, TResponse>(TRequest request)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public interface IHandlerSetup<TRequest, TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public Dispatch<TRequest, TResult, TResponse> GetDispatcher
    (
        IServiceProvider provider,
        Dispatch<TRequest, TResult, TResponse> dispatch
    );
}

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options(Func<IServiceProvider, Dispatch> DispatchFactory, ServiceLifetime ServiceLifetime)
    {
        public Func<IServiceProvider, IHandlerSetup<TRequest, TResult, TResponse>> HandlerFactory { get; set; } = DefautlHandlerSetup;

        private readonly List<Action<IServiceCollection>> _extensions = [];

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        public static IHandlerSetup<TRequest, TResult, TResponse> DefautlHandlerSetup(IServiceProvider provider)
        {
            return provider.GetRequiredService<IHandlerSetup<TRequest, TResult, TResponse>>();
        }

        public void Register
        (
            IServiceCollection services
        )
        {
            foreach(var extension in _extensions)
            {
                extension(services);
            }
            
            services.Add(ServiceDescriptor.Describe(typeof(Dispatch), GetDispatch, ServiceLifetime));

            Dispatch GetDispatch(IServiceProvider provider)
            {
                Dispatch<TRequest, TResult, TResponse> dispatch = request => DispatchFactory(provider)(request);

                dispatch = HandlerFactory(provider).GetDispatcher(provider, dispatch);
                
                return request => dispatch(request);
            }
        }
    }
}