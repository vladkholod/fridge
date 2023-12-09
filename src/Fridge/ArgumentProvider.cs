namespace Fridge;

public class ArgumentProvider
{
    private readonly Dictionary<string, string> _arguments = new();

    public ArgumentProvider(IReadOnlyList<string> args)
    {
        Initialize(args);
        
        Validate();
    }

    public string GetValue(string argument)
    {
        return _arguments[argument];
    }

    private void Initialize(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
        {
            return;
        }

        if (args.Count % 2 != 0)
        {
            throw new AggregateException("Invalid count of arguments are provided.");
        }

        for (var index = 0; index < args.Count; index += 2)
        {
            var key = args[index];
            var value = args[index + 1];

            _arguments.TryAdd(key, value);
        }
    }
    
    private void Validate()
    {
        // var containsMissingArguments = false;
        //
        // foreach (var argument in Arguments.All)
        // {
        //     if (_arguments.ContainsKey(argument)) continue;
        //
        //     Console.WriteLine($"{argument} is not provided.");
        //     containsMissingArguments = true;
        // }
        //
        // if (containsMissingArguments)
        // {
        //     throw new AggregateException(
        //         $"Not all arguments is provided. Required arguments: {string.Join(",", Arguments.All)}");
        // }
    }
}