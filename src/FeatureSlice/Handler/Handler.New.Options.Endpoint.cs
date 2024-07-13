using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureSlice;

public interface IEndpointBuilder : IEndpointConventionBuilder
{
    public void Map(IEndpointRouteBuilder builder);

    public static async Task MapAll(IEndpointRouteBuilder builder)
    {
        await using var scope = builder.ServiceProvider.CreateAsyncScope();

        var endpoints = scope.ServiceProvider.GetServices<IEndpointBuilder>();

        foreach(var endpoint in endpoints)
        {
            endpoint.Map(builder);
        }
    }
}

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record Options
    {
        public sealed partial record Endpoint(Options Options, Func<IEndpointRouteBuilder, IEndpointConventionBuilder> Extender) : IEndpointBuilder
        {
            private readonly List<Action<EndpointBuilder>> _conventions = [];

            public void Add(Action<EndpointBuilder> convention)
            {
                _conventions.Add(convention);
            }

            void IEndpointBuilder.Map(IEndpointRouteBuilder builder)
            {
                var endpointConventionBuilder = Extender(builder);
                foreach (var convention in _conventions)
                {
                    endpointConventionBuilder.Add(convention);
                }
            }

            public void TryRegister()
            {
                Options.Extend(services => services.TryAddEnumerable(ServiceDescriptor.Singleton<IEndpointBuilder>(this)));
            }

            public static implicit operator Options(Endpoint endpoint)
            {
                endpoint.TryRegister();
                return endpoint.Options;
            }
        }
    }
}