﻿using FeatureSlice;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace Samples;

internal static partial class ExampleFeature
{
    public sealed record Request();
    public sealed record Response();

    public sealed partial class FeatureSlice1 : IFeatureSlice<Request, Response>, IRegistrable
    {
        public static string FeatureName => "FeatureSlice1";

        public Task<OneOf<Response, Error>> Handle(Request request)
        {
            throw new Exception();
        }
    }

    public sealed partial class FeatureSlice2 : IFeatureSlice<Request, Response>, IRegistrable
    {
        public static string FeatureName => "FeatureSlice2";

        public Task<OneOf<Response, Error>> Handle(Request request)
        {
            throw new Exception();
        }
    }
}

internal static partial class ExampleFeature
{
    public static void Register(IServiceCollection services)
    {
        services.Register<FeatureSlice1>();
        services.Register<FeatureSlice2>();
    }

    public static async Task Run(FeatureSlice1.Dispatch slice1, FeatureSlice2.Dispatch slice2, Publisher<Request>.Dispatch dispatch)
    {
        var result1 = await slice1(new Request());
        var result2 = await slice2(new Request());
        var result3 = await dispatch(new Request());

        result2.Switch(
            resposnse => {},
            disabled => {},
            error => {});
    }
}

//Autogeneratoed
internal static partial class ExampleFeature
{
    public sealed partial class FeatureSlice1
    {
        public delegate Task<OneOf<Response, Disabled, Error>> Dispatch(Request request);

        public static void Register(IServiceCollection services)
        {
            IFeatureSlice<Request, Response>.Setup<FeatureSlice1>.Register<Dispatch>(
                services,
                provider => request => IFeatureSlice<Request, Response>.Setup<FeatureSlice1>.DispatchFactory(provider).Invoke(request));
        }
    }

    public sealed partial class FeatureSlice2
    {
        public delegate Task<OneOf<Response, Disabled, Error>> Dispatch(Request request);

        public static void Register(IServiceCollection services)
        {
            IFeatureSlice<Request, Response>.Setup<FeatureSlice2>.Register<Dispatch>(
                services,
                provider => request => IFeatureSlice<Request, Response>.Setup<FeatureSlice2>.DispatchFactory(provider).Invoke(request));
        }
    }
}