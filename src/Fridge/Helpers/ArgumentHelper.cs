namespace Fridge.Helpers;

public static class ArgumentHelper
{
    public static bool IsArgument(string value)
    {
        return value.StartsWith('-');
    }
}