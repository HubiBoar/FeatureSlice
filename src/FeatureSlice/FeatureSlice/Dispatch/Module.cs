using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

namespace FeatureSlice.Dispatch;

public interface IApplicationSetup
{
    public IServiceCollection Services { get; }

    public IConfigurationManager Configuration { get; }
}

public record ModuleSetup<THost>(IServiceCollection Services, IConfigurationManager Configuration) : IApplicationSetup
    where THost : IHost
{
    public void AddHostExtension(HostExtension<THost> extension)
    {
        Services.AddHostExtension(extension);
    }
}

public interface IHostExtender<THost> : IApplicationSetup
    where THost : IHost
{
    public void AddHostExtension(HostExtension<THost> extension)
    {
        Services.AddSingleton(extension);
    }
}

public abstract class Module<THost> : IHostExtender<THost>
    where THost : IHost
{
    public ModuleSetup<THost> Setup { get; }

    public IServiceCollection Services => Setup.Services;

    public IConfigurationManager Configuration => Setup.Configuration;

    protected Module(ModuleSetup<THost> setup)
    {
        Setup = setup;
    }
}

public delegate void HostExtension<THost>(THost extension)
    where THost : IHost;

public static class ServiceCollectionExtensions
{
    public static TModule AddModule<TModule>(this WebApplicationBuilder builder, Func<ModuleSetup<WebApplication>, TModule> setup)
    {
        return setup(new ModuleSetup<WebApplication>(builder.Services, builder.Configuration));
    }

    public static IServiceCollection AddHostExtension<THost>(this IServiceCollection services, HostExtension<THost> extension)
        where THost : IHost
    {
        services.AddSingleton(extension);

        return services;
    }

    public static T Extend<T>(this T host)
        where T : IHost
    {
        foreach(var extension in host.Services.GetServices<HostExtension<T>>())
        {
            extension.Invoke(host);
        }

        foreach(var extension in host.Services.GetServices<HostExtension<IHost>>())
        {
            extension.Invoke(host);
        }

        return host;
    }
}


public sealed record WebHost(IWebHost Host) : IHost
{
    public IServiceProvider Services => Host.Services;

    public void Dispose()
    {
        Host.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Host.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Host.StopAsync(cancellationToken);
    }
}

internal sealed record InternalConfigurationManager(IConfigurationBuilder Builder) : IConfigurationManager
{
    public string? this[string key] { get => Configuration[key]; set => Configuration[key] = value; }

    public IConfigurationRoot Configuration => Build();

    public IDictionary<string, object> Properties => Builder.Properties;

    public IList<IConfigurationSource> Sources => Builder.Sources;

    public IConfigurationBuilder Add(IConfigurationSource source)
    {
        return Builder.Add(source);
    }

    public IConfigurationRoot Build()
    {
        return Builder.Build();
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return Configuration.GetChildren();
    }

    public IChangeToken GetReloadToken()
    {
        return Configuration.GetReloadToken();
    }

    public IConfigurationSection GetSection(string key)
    {
        return Configuration.GetSection(key);
    }
}

public static class HostExtensions
{
    public static TModule AddModule<TModule>(this HostBuilder builder, Func<ModuleSetup<IHost>, TModule> setup)
    {
        IServiceCollection _services = null!;
        IConfigurationBuilder _configurationBuilder = null!;

        builder.ConfigureServices(services => {
            _services = services;
        });

        builder.ConfigureAppConfiguration(config => {
            _configurationBuilder = config;
        });

        var configurationManager = new InternalConfigurationManager(_configurationBuilder);

        return setup(new ModuleSetup<IHost>(_services, configurationManager));
    }

    public static TModule AddModule<TModule>(this WebHostBuilder builder, Func<ModuleSetup<WebHost>, TModule> setup)
    {
        IServiceCollection _services = null!;
        IConfigurationBuilder _configurationBuilder = null!;

        builder.ConfigureServices(services => {
            _services = services;
        });

        builder.ConfigureAppConfiguration(config => {
            _configurationBuilder = config;
        });

        var configurationManager = new InternalConfigurationManager(_configurationBuilder);

        return setup(new ModuleSetup<WebHost>(_services, configurationManager));
    }

    public static T Extend<T>(this T host)
        where T : IWebHost
    {
        var webHost = new WebHost(host);

        foreach(var extension in host.Services.GetServices<HostExtension<WebHost>>())
        {
            extension.Invoke(webHost);
        }

        foreach(var extension in host.Services.GetServices<HostExtension<IHost>>())
        {
            extension.Invoke(webHost);
        }

        return host;
    }
}