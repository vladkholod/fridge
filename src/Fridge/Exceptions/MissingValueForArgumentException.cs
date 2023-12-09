using Fridge.Models;

namespace Fridge.Exceptions;

public class MissingValueForArgumentException : Exception
{
    public MissingValueForArgumentException(Argument argument)
        : base($"Missing value for argument {argument.Full}.")
    {
    }
}