using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace FeatureSlice;

public sealed class HostExtender<THost>
    where THost : IHost
{
    private readonly List<Action<THost>> _extensions = new List<Action<THost>>();

    public void AddExtension(Action<THost> extension)
    {
        _extensions.Add(extension);
    }

    public void AddExtension(Action<IHost> extension)
    {
        _extensions.Add(host => extension(host));
    }

    public void Extend(THost host)
    {
        foreach(var extension in _extensions)
        {
            extension(host);
        }
    }
}