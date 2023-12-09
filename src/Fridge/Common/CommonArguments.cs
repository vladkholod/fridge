using Fridge.Models;

namespace Fridge.Common;

public static class CommonArguments
{
    public static readonly Argument Help = new()
    {
        Full = "--help",
        Short = "-h",
        RequiresValue = false,
    };
}