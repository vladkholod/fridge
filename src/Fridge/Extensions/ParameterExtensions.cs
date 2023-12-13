using Fridge.Models;

namespace Fridge.Extensions;

public static class ParameterExtensions
{
    public static bool ContainsArgument(this IEnumerable<Parameter> parameters, Argument argument)
    {
        return parameters.Any(parameter => parameter.Argument.Equals(argument));
    }
}