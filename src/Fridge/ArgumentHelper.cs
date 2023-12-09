using Fridge.Common;
using Fridge.Models;

namespace Fridge;

public static class ArgumentHelper
{
    public static bool IsHelp(Argument[] arguments)
    {
        return arguments.Length == 1 && arguments[0].Equals(CommonArguments.Help);
    }

    public static bool IsArgument(string value)
    {
        return value.StartsWith('-');
    }
}