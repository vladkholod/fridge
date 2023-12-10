using Fridge.Commands;

namespace Fridge.Models;

public class CommandDescriptor
{
    public CommandDescriptor(ICommand command, Parameter[]? parameters = null)
    {
        Command = command;
        Parameters = parameters ?? Array.Empty<Parameter>();
    }

    public ICommand Command { get; }

    public Parameter[] Parameters { get; }
}