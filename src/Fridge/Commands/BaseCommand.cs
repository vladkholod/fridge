using System.Text;
using Fridge.Common;
using Fridge.Exceptions;
using Fridge.Models;

namespace Fridge.Commands;

public abstract class BaseCommand : ICommand
{
    protected BaseCommand(CommandConfig config)
    {
        Config = config;
    }

    public CommandConfig Config { get; }

    public abstract void Execute(Parameter[] parameters);

    public StringBuilder GetHelpBuilder()
    {
        var helpBuilder = new StringBuilder(string.Format(Config.DescriptionFormat, Config.Name));
        helpBuilder.AppendLine();

        if (Config.RequiredArguments.Length != 0)
        {
            AppendSection("Required arguments", Config.RequiredArguments);
        }

        if (Config.RequiredArguments.Length != 0)
        {
            AppendSection("Optional arguments", Config.OptionalArguments);
        }

        return helpBuilder;

        void AppendSection(string header, IEnumerable<Argument> arguments)
        {
            helpBuilder.Append(header);
            helpBuilder.AppendLine(":");

            foreach (var argument in arguments)
            {
                helpBuilder.AppendLine($"\t{argument.Full} or {argument.Short} - {argument.Description}");
            }
        }
    }

    public void ValidateRequiredArguments(Argument[] arguments)
    {
        var missingRequiredArguments = Config.RequiredArguments.Except(arguments)
            .ToArray();

        if (missingRequiredArguments.Length != 0)
        {
            throw new MissingRequiredArgumentsException(Config.Name, missingRequiredArguments);
        }
    }
}