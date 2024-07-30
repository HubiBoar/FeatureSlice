using Definit.Results;

namespace FeatureSlice;

public static partial class Arg
{
    public sealed record Runner
    (
        Command Command,
        IReadOnlyCollection<Option> options,
        Func<string[], Task<Result<bool>>> Job
    );

    public sealed record Continue();
    public delegate Result<string, Continue> TryGet(string[] args, Command command);

    public sealed record Command(string Name);
    public sealed record Option(string Helper, TryGet TryGet);

    public static Command Cmd(string name) => new (name);
    public static Option Opt(string name, string shortcut) => new
    (
        $"--{name}, -{shortcut}",
        (args, command) =>
        {
            return TryGetArgsValue(args, command, [$"--{name}", $"-{shortcut}"]);
        }
    );

    private static Result<string, Continue> TryGetArgsValue(string[] args, Command command, string[] names)
    {
        try
        {
            if(args.All(x => command.Name == x == false))
            {
                return new Continue();
            }

            int? argIndex = args
                .Select((x, i) => (Value: x, Index: i))
                .Where(x => names.Contains(x.Value))
                .Select(x => x.Index)
                .SingleOrDefault();

            if(argIndex is null)
            {
                return new Continue();
            }

            return args[argIndex.Value + 1];
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
}