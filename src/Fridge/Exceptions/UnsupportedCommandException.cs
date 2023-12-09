namespace Fridge.Exceptions;

public class UnsupportedCommandException : Exception
{
    public UnsupportedCommandException(string commandName)
        : base($"Unsupported command is received: `{commandName}`.")
    {
    }
}