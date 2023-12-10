using Fridge.Commands;
using Fridge.Commands.Help;
using Fridge.Exceptions;
using Fridge.Helpers;
using Fridge.Models;

namespace Fridge.Services;

public interface IArgsParser
{
    CommandDescriptor Parse(string[] args);
}

public class ArgsParser : IArgsParser
{
    private readonly ICommandRegistry _commandRegistry;

    public ArgsParser(ICommandRegistry commandRegistry)
    {
        _commandRegistry = commandRegistry;
    }

    public CommandDescriptor Parse(string[] args)
    {
        if (args.Length != 0)
        {
            return GetCommandDescriptor(args);
        }
        
        var helpCommand = _commandRegistry.Get(HelpCommand.CommandConfig.Name);
        return new CommandDescriptor(helpCommand);
    }

    private CommandDescriptor GetCommandDescriptor(string[] args)
    {
        var command = _commandRegistry.Get(args[0]);

        if (args.Length == 1)
        {
            return new CommandDescriptor(command);
        }

        var parameters = GetParameters(command, args[1..]).ToArray();

        return new CommandDescriptor(command, parameters);
    }

    private static IEnumerable<Parameter> GetParameters(ICommand command, string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var argumentName = args[i];
            var argument = command.Config.AllArguments.FirstOrDefault(argument => argument.Equals(argumentName));
            if (argument is null)
            {
                throw new UnsupportedArgumentException(command.Config.Name, argumentName);
            }

            string? value = null;
            if (argument.RequiresValue)
            {
                value = GetValue(ref i);

                if (value is null)
                {
                    throw new MissingValueForArgumentException(argument);
                }
            }

            yield return new Parameter(argument, value);
        }

        yield break;

        string? GetValue(ref int current)
        {
            var next = current + 1;
            if (next >= args.Length)
            {
                return null;
            }

            var value = args[next];
            if (ArgumentHelper.IsArgument(value))
            {
                return null;
            }

            current = next;
            return value;
        }
    }
}