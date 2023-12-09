using System.Text;
using Fridge.Models;

namespace Fridge.Exceptions;

public class MissingRequiredArgumentsException : Exception
{
    public MissingRequiredArgumentsException(string commandName, Argument[] missingRequiredArguments)
        : base($"Invalid executing arguments are passed for `{commandName}` command.")
    {
        CommandName = commandName;
        MissingRequiredArguments = missingRequiredArguments;
    }

    public string CommandName { get; }
    public Argument[] MissingRequiredArguments { get; }

    public override string ToString()
    {
        var messageBuilder = new StringBuilder(Message);
        messageBuilder.AppendLine();

        if (MissingRequiredArguments.Length != 0)
        {
            AppendList("Required arguments are missing", MissingRequiredArguments);
        }

        return messageBuilder.ToString();

        void AppendList(string header, IEnumerable<Argument> arguments)
        {
            messageBuilder.AppendLine();
            messageBuilder.Append(header);
            messageBuilder.AppendLine(":");

            foreach (var argument in arguments)
            {
                messageBuilder.AppendLine($"\t{argument}");
            }
        }
    }
}