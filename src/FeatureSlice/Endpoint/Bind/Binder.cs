using Microsoft.AspNetCore.Http;

namespace FeatureSlice;

public interface IAnyBinder<T> : ILastBinder<T>
{
}

public interface ILastBinder<T> : IBinder<T>
{
}

public interface IBinder<T>
{
    ValueTask<T> BindAsync(HttpContext context);

    void ExtendEndpoint(IEndpointBuilder builder);
}