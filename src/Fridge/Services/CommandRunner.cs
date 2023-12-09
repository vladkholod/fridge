using System.Text;
using Fridge.Commands;
using Fridge.Commands.Freeze;
using Fridge.Common;
using Fridge.Exceptions;
using Fridge.Models;

namespace Fridge.Services;

public interface ICommandRunner
{
    void Run(string[] args);
}

public class CommandRunner : ICommandRunner
{
    private static readonly IReadOnlyDictionary<string, ICommand> Commands = new Dictionary<string, ICommand>
    {
        { FreezeCommand.CommandConfig.Name, new FreezeCommand() }
    };

    public void Run(string[] args)
    {
        if (args.Length == 0)
        {
            DisplayGlobalHelp();
            return;
        }

        var (command, parameters) = GetCommandWithParameters(args);

        if (IsHelpRequested(parameters))
        {
            var commandHelpBuilder = command.GetHelpBuilder();
            Console.WriteLine(commandHelpBuilder);
            return;
        }

        command.ValidateRequiredArguments(parameters.Select(parameter => parameter.Argument).ToArray());

        command.Execute(parameters);
    }

    private static (ICommand Command, Parameter[] Parameters) GetCommandWithParameters(string[] args)
    {
        var command = GetCommand(args[0]);

        if (args.Length == 1)
        {
            return (command, Array.Empty<Parameter>());
        }

        var parameters = GetParameters(command, args[1..]).ToArray();

        return (command, parameters);
    }

    private static ICommand GetCommand(string commandName)
    {
        if (!Commands.TryGetValue(commandName, out var command))
        {
            throw new UnsupportedCommandException(commandName);
        }

        return command;
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

    private static void DisplayGlobalHelp()
    {
        var globalHelpBuilder = new StringBuilder("Fridge supports the following commands:")
            .AppendLine();

        foreach (var command in Commands.Values)
        {
            var commandHelpBuilder = command.GetHelpBuilder();
            globalHelpBuilder.Append(commandHelpBuilder);
        }
        
        Console.WriteLine(globalHelpBuilder);
    }

    private static bool IsHelpRequested(Parameter[] parameters)
    {
        return parameters.Any(parameter => parameter.Argument.Equals(CommonArguments.Help));
    }
}