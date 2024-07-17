using Definit.Results;

namespace FeatureSlice;

public static class FeatureSliceRequestExtensions
{
    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        ILastBinder<T0> bind0,
        Func<T0, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);

            return await mapRequest(value0);
        },
        bind0.ExtendEndpoint));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        ILastBinder<T0> bind0,
        Func<T0, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return Request(builder, bind0, v0 => Task.FromResult(mapRequest(v0)));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        ILastBinder<T1> bind1,
        Func<T0, T1, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {   
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);
            var value1 = await bind1.BindAsync(context);

            return await mapRequest(value0, value1);
        },
        endpoint => 
        {
            bind0.ExtendEndpoint(endpoint);
            bind1.ExtendEndpoint(endpoint);
        }));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        ILastBinder<T1> bind1,
        Func<T0, T1, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {   
        return Request(builder, bind0, bind1, (v0, v1) => Task.FromResult(mapRequest(v0, v1)));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        ILastBinder<T2> bind2,
        Func<T0, T1, T2, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {   
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);
            var value1 = await bind1.BindAsync(context);
            var value2 = await bind2.BindAsync(context);

            return await mapRequest(value0, value1, value2);
        },
        endpoint => 
        {
            bind0.ExtendEndpoint(endpoint);
            bind1.ExtendEndpoint(endpoint);
            bind2.ExtendEndpoint(endpoint);
        }));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        ILastBinder<T2> bind2,
        Func<T0, T1, T2, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return Request(builder, bind0, bind1, bind2, (v0, v1, v2) => Task.FromResult(mapRequest(v0, v1, v2)));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2, T3>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        IAnyBinder<T2> bind2,
        ILastBinder<T3> bind3,
        Func<T0, T1, T2, T3, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);
            var value1 = await bind1.BindAsync(context);
            var value2 = await bind2.BindAsync(context);
            var value3 = await bind3.BindAsync(context);

            return await mapRequest(value0, value1, value2, value3);
        },
        endpoint => 
        {
            bind0.ExtendEndpoint(endpoint);
            bind1.ExtendEndpoint(endpoint);
            bind2.ExtendEndpoint(endpoint);
            bind3.ExtendEndpoint(endpoint);
        }));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2, T3>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        IAnyBinder<T2> bind2,
        ILastBinder<T3> bind3,
        Func<T0, T1, T2, T3, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return Request(builder, bind0, bind1, bind2, bind3, (v0, v1, v2, v3) => Task.FromResult(mapRequest(v0, v1, v2, v3)));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2, T3, T4>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        IAnyBinder<T2> bind2,
        IAnyBinder<T3> bind3,
        ILastBinder<T4> bind4,
        Func<T0, T1, T2, T3, T4, Task<TRequest>> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return new (builder, new (async context => 
        {
            var value0 = await bind0.BindAsync(context);
            var value1 = await bind1.BindAsync(context);
            var value2 = await bind2.BindAsync(context);
            var value3 = await bind3.BindAsync(context);
            var value4 = await bind4.BindAsync(context);

            return await mapRequest(value0, value1, value2, value3, value4);
        },
        endpoint => 
        {
            bind0.ExtendEndpoint(endpoint);
            bind1.ExtendEndpoint(endpoint);
            bind2.ExtendEndpoint(endpoint);
            bind3.ExtendEndpoint(endpoint);
            bind4.ExtendEndpoint(endpoint);
        }));
    }

    public static ResponseBuilder<TRequest, TResult, TResponse> Request<TRequest, TResult, TResponse, T0, T1, T2, T3, T4>
    (
        this EndpointBuilder<TRequest, TResult, TResponse> builder,
        IAnyBinder<T0> bind0,
        IAnyBinder<T1> bind1,
        IAnyBinder<T2> bind2,
        IAnyBinder<T3> bind3,
        ILastBinder<T4> bind4,
        Func<T0, T1, T2, T3, T4, TRequest> mapRequest
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return Request(builder, bind0, bind1, bind2, bind3, bind4, (v0, v1, v2, v3, v4) => Task.FromResult(mapRequest(v0, v1, v2, v3, v4)));
    }
}