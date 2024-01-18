using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.Dispatch;

public partial interface IRegistrable<TSetup>
    where TSetup : IApplicationSetup
{
    public static abstract void Register(TSetup setup);
}

public partial interface IRegistrable : IRegistrable<IApplicationSetup>
{
    public interface IWebApp : IRegistrable<IApplicationSetup<WebApplication>>
    {
    }

    public interface IWebHost : IRegistrable<IApplicationSetup<WebHost>>
    {
    }

    public interface IHost : IRegistrable<IApplicationSetup<Microsoft.Extensions.Hosting.IHost>>
    {
    }
}