using Fridge.Commands;
using Fridge.Exceptions;

namespace Fridge.Services;

public interface ICommandRegistry
{
    IEnumerable<ICommand> All { get; }

    bool Register(ICommand command);

    ICommand Get(string name);
}

public class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, ICommand> _commands = new();

    public IEnumerable<ICommand> All => _commands.Values;

    public ICommand Get(string name)
    {
        if (!_commands.TryGetValue(name, out var command))
        {
            throw new UnsupportedCommandException(name);
        }

        return command;
    }

    public bool Register(ICommand command)
    {
        var isAdded = _commands.TryAdd(command.Config.Name, command);

        return isAdded;
    }
}