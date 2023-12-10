using System.Text;
using Fridge.Models;
using Fridge.Services;

namespace Fridge.Commands.Help;

public class HelpCommand : BaseCommand
{
    private readonly ICommandRegistry _commandRegistry;

    public static readonly CommandConfig CommandConfig = new()
    {
        Name = "help",
        DescriptionFormat = "The `{0}` command displays help for the whole util."
    };
    
    public HelpCommand(ICommandRegistry commandRegistry) : base(CommandConfig)
    {
        _commandRegistry = commandRegistry;
    }

    public override void Execute(Parameter[] parameters)
    {
        var utilHelpBuilder = new StringBuilder("Fridge supports the following commands:")
            .AppendLine();

        foreach (var command in _commandRegistry.All)
        {
            var commandHelpBuilder = command.GetHelpBuilder();
            utilHelpBuilder.Append(commandHelpBuilder);
        }

        Console.WriteLine(utilHelpBuilder);
    }
}