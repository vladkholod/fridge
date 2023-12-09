namespace Fridge.Exceptions;

public class UnsupportedArgumentException : Exception
{
    public UnsupportedArgumentException(string commandName, string argumentName)
        : base($"Unsupported argument for command {commandName} is received: `{argumentName}`.")
    {
    }
}