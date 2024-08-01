using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice;

public interface IFeatureSlice
{
    protected void Configure(IServiceProvider provider);
    protected ServiceLifetime RunExtensions(IServiceCollection services);

    public static void Register<T>(IServiceCollection services)
        where T : class, IFeatureSlice, new()
    {
        var slice = new T();
        var lifetime = slice.RunExtensions(services);

        services.Add
        (
            ServiceDescriptor.Describe
            (
                typeof(T),
                    provider =>
                {
                    var slice = new T();

                    slice.Configure(provider);

                    return slice;
                },
                lifetime
            )
        );
    }
}

public interface IFeatureSlice<THandle> : IFeatureSlice
    where THandle : Delegate
{
    public THandle Dispatch { get; protected set; }

    protected Options Opts { get; }

    public sealed record Options
    (
        Func<IServiceProvider, THandle> GetHandle,
        ServiceLifetime ServiceLifetime = ServiceLifetime.Transient
    )
    {
        public required Func<IServiceProvider, THandle, THandle> Dispatcher { get; set; }

        private readonly List<Action<IServiceCollection>> _extensions = [];

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        internal ServiceLifetime RunExtensions(IServiceCollection services)
        {
            foreach(var extension in _extensions)
            {
                extension(services);
            }

            return ServiceLifetime;
        }
    }

    void IFeatureSlice.Configure(IServiceProvider provider)
    {
        Dispatch = Opts.GetHandle(provider);
    }

    ServiceLifetime IFeatureSlice.RunExtensions(IServiceCollection services)
    {
        return Opts.RunExtensions(services);
    }

    public static T CreateForTest<T>(THandle handle)
        where T : class, IFeatureSlice<THandle>, new()
    {
        return new T()
        {
            Dispatch = handle
        };
    }
}

public abstract partial record FeatureSliceBase<TRequest, TResult, TResponse>
(
    IFeatureSlice<Dispatch<TRequest, TResult, TResponse>>.Options Opts
)
: IFeatureSlice<Dispatch<TRequest, TResult, TResponse>>

where TRequest : notnull
where TResult : Result_Base<TResponse>
where TResponse : notnull

{
    public Dispatch<TRequest, TResult, TResponse> Dispatch { get; private set; } = null!;

    Dispatch<TRequest, TResult, TResponse> IFeatureSlice<Dispatch<TRequest, TResult, TResponse>>.Dispatch
    {
        get => Dispatch;
        set => Dispatch = value;
    }

    public static T CreateForTest<T>
    (
        Dispatch<TRequest, TResult, TResponse> handle
    )
        where T : FeatureSliceBase<TRequest, TResult, TResponse>, new()
    {
        return IFeatureSlice<Dispatch<TRequest, TResult, TResponse>>.CreateForTest<T>(handle);
    }
}