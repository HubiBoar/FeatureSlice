using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options(Func<IServiceProvider, Dispatch> DispatchFactory)
    {
        private readonly List<Action<IServiceCollection>> _extensions = [];

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        public void Register
        (
            IServiceCollection services,
            Func<IServiceProvider, Dispatch, Dispatch> dispatchModifier,
            ServiceLifetime serviceLifetime
        )
        {
            foreach(var extension in _extensions)
            {
                extension(services);
            }
            
            services.Add(ServiceDescriptor.Describe(typeof(Dispatch), GetDispatch, serviceLifetime));

            Dispatch GetDispatch(IServiceProvider provider)
            {
                var dispatch = DispatchFactory(provider); 
                return dispatchModifier(provider, dispatch);
            }
        }
    }
}