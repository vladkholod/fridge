using Fridge.Commands.Freeze;
using Fridge.Commands.Help;
using Fridge.Services;

namespace Fridge;

internal static class Program
{
    public static void Main(string[] args)
    {
        ICommandRegistry commandRegistry = new CommandRegistry();
        commandRegistry.Register(new FreezeCommand());
        commandRegistry.Register(new HelpCommand(commandRegistry));
        
        IArgsParser argsParser = new ArgsParser(commandRegistry);
        var commandDescriptor = argsParser.Parse(args);
        
        ICommandRunner commandRunner = new CommandRunner();
        commandRunner.Run(commandDescriptor);
    }
}