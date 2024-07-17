using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IHandlerOptions<TRequest, TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{

}

public delegate Task<TResult> Dispatch<TRequest, TResult, TResponse>(TRequest request)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public interface IDispatcher<TRequest, TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public Dispatch<TRequest, TResult, TResponse> GetDispatch
    (
        IServiceProvider provider,
        Dispatch<TRequest, TResult, TResponse> dispatch
    );
}

public abstract partial class HandlerBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options(Func<IServiceProvider, Dispatch> DispatchFactory, ServiceLifetime ServiceLifetime)
    {
        public static IDispatcher<TRequest, TResult, TResponse> DefautlHandlerSetup(IServiceProvider provider)
        {
            return provider.GetRequiredService<IDispatcher<TRequest, TResult, TResponse>>();
        }

        public void Register
        (
            IServiceCollection services,
            Func<IServiceProvider, IDispatcher<TRequest, TResult, TResponse>> setupFactory
        )
        {
            services.Add(ServiceDescriptor.Describe(typeof(Dispatch), GetDispatch, ServiceLifetime));

            Dispatch GetDispatch(IServiceProvider provider)
            {
                Dispatch<TRequest, TResult, TResponse> dispatch = request => DispatchFactory(provider)(request);

                dispatch = setupFactory(provider).GetDispatch(provider, dispatch);
                
                return request => dispatch(request);
            }
        }
    }
}