using Microsoft.Extensions.DependencyInjection;
using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
            services.Add(serviceLifetime, GetDispatch);

            Dispatch GetDispatch(IServiceProvider provider)
            {
                var dispatch = DispatchFactory(provider); 
                return dispatchModifier(provider, dispatch);
            }
        }
    }
}