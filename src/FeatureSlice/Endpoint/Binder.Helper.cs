using Microsoft.AspNetCore.Http;

namespace FeatureSlice;

public static class Binder
{
    public static FromBodyJsonBinder<T> FromBodyJson<T>()
        where T : notnull
    {
        return new ();
    }

    public static FromQueryBinder<T> FromQuery<T>(string name, bool required = true)
    {
        return new (name, required);
    }

    public static FromRouteBinder<T> FromRoute<T>(string name, bool required = true)
    {
        return new (name, required);
    }

    public static FromHeaderBinder<T> FromHeader<T>(string name, bool required = true)
    {
        return new (name, required);
    }

    public static FromCookieBinder<T> FromCookie<T>(string name, bool required = true)
    {
        return new (name, required);
    }
}

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