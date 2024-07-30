using Definit.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneOf.Else;

namespace FeatureSlice;

public static class FeatureSliceConsoleExtensions
{
    public static FeatureSliceOptions MapCli(this FeatureSliceOptions options, string[] args)
    {
        options.Services.AddFeatureSlicesExtension(async (IHost host, IServiceProvider  provider) =>
        {
            var runners = provider.GetServices<Arg.Runner>();
            foreach(var runner in runners)
            {
                Console.WriteLine(runner.Command.Name);
            
                foreach(var option in runner.options)
                {
                    Console.WriteLine($"\t{option.Helper}");
                }

                await runner.Job(args);
            }
        });

        return options;
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapCli<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup setup,
        Arg.Command command,
        Arg.Option option0,
        Func<string, TRequest> argsMapper
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        setup.Extend(services => services.AddSingleton(provider => 
        {
            var dispatcher = setup.GetDispatch(provider);

            return new Arg.Runner(command, [option0], async (args) =>
            {
                try
                {
                    if(option0.TryGet(args, command).Is(out Arg.Continue _).Else(out var reminder0))
                    {
                        return false;
                    }
                    if(reminder0.Is(out Error error).Else(out var arg0))
                    {
                        return error;
                    }

                    var request = argsMapper(arg0);
                    var result = await dispatcher(request);

                    if(result.Is(out error))
                    {
                        return error;
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    return exception;
                }
            });
        }));

        return setup;
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapCli<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup setup,
        Arg.Command command,
        Arg.Option option0,
        Arg.Option option1,
        Func<string, string, TRequest> argsMapper
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        setup.Extend(services => services.AddSingleton(provider => 
        {
            var dispatcher = setup.GetDispatch(provider);

            return new Arg.Runner(command, [option0, option1], async (args) =>
            {
                try
                {
                    if(option0.TryGet(args, command).Is(out Arg.Continue _).Else(out var reminder0))
                    {
                        return false;
                    }
                    if(reminder0.Is(out Error error).Else(out var arg0))
                    {
                        return error;
                    }

                    if(option1.TryGet(args, command).Is(out Arg.Continue _).Else(out var reminder1))
                    {
                        return false;
                    }
                    if(reminder1.Is(out error).Else(out var arg1))
                    {
                        return error;
                    }

                    var request = argsMapper(arg0, arg1);
                    var result = await dispatcher(request);

                    if(result.Is(out error))
                    {
                        return error;
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    return exception;
                }
            });
        }));

        return setup;
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapCli<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup setup,
        Arg.Command command,
        Arg.Option option0,
        Arg.Option option1,
        Arg.Option option2,
        Func<string, string, string, TRequest> argsMapper
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        setup.Extend(services => services.AddSingleton(provider => 
        {
            var dispatcher = setup.GetDispatch(provider);

            return new Arg.Runner(command, [option0, option1], async (args) =>
            {
                try
                {
                    if(option0.TryGet(args, command).Is(out Arg.Continue _).Else(out var reminder0))
                    {
                        return false;
                    }
                    if(reminder0.Is(out Error error).Else(out var arg0))
                    {
                        return error;
                    }

                    if(option1.TryGet(args, command).Is(out Arg.Continue _).Else(out var reminder1))
                    {
                        return false;
                    }
                    if(reminder1.Is(out error).Else(out var arg1))
                    {
                        return error;
                    }

                    if(option2.TryGet(args, command).Is(out Arg.Continue _).Else(out var reminder2))
                    {
                        return false;
                    }
                    if(reminder2.Is(out error).Else(out var arg2))
                    {
                        return error;
                    }

                    var request = argsMapper(arg0, arg1, arg2);
                    var result = await dispatcher(request);

                    if(result.Is(out error))
                    {
                        return error;
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    return exception;
                }
            });
        }));

        return setup;
    }
}