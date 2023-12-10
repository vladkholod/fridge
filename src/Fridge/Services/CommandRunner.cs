using Fridge.Commands;
using Fridge.Common;
using Fridge.Models;

namespace Fridge.Services;

public interface ICommandRunner
{
    void Run(CommandDescriptor commandDescriptor);
}

public class CommandRunner : ICommandRunner
{
    public void Run(CommandDescriptor commandDescriptor)
    {
        if (IsHelpRequested(commandDescriptor.Parameters))
        {
            DisplayCommandHelp(commandDescriptor.Command);
            return;
        }

        ValidateRequiredParameters(commandDescriptor);

        ExecuteCommand(commandDescriptor);
    }

    private static void DisplayCommandHelp(ICommand command)
    {
        var commandHelpBuilder = command.GetHelpBuilder();

        Console.WriteLine(commandHelpBuilder);
    }

    private static void ValidateRequiredParameters(CommandDescriptor commandDescriptor)
    {
        var commandArguments = commandDescriptor.Parameters.Select(parameter => parameter.Argument).ToArray();

        commandDescriptor.Command.ValidateRequiredArguments(commandArguments);
    }

    private static void ExecuteCommand(CommandDescriptor commandDescriptor)
    {
        commandDescriptor.Command.Execute(commandDescriptor.Parameters);
    }

    private static bool IsHelpRequested(IEnumerable<Parameter> parameters)
    {
        return parameters.Any(parameter => parameter.Argument.Equals(CommonArguments.Help));
    }
}