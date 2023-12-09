using Fridge.Common;
using Fridge.Models;

namespace Fridge.Commands;

public class CommandConfig
{
    private Argument[]? _allArguments;

    public string Name { get; init; } = null!;

    public string DescriptionFormat { get; init; } = null!;

    public Argument[] RequiredArguments { get; init; } = Array.Empty<Argument>();

    public Argument[] OptionalArguments { get; init; } = Array.Empty<Argument>();

    public Argument[] AllArguments => _allArguments ??= GetAllArguments().ToArray();

    private IEnumerable<Argument> GetAllArguments()
    {
        foreach (var requiredArgument in RequiredArguments)
        {
            yield return requiredArgument;
        }

        foreach (var optionalArgument in OptionalArguments)
        {
            yield return optionalArgument;
        }

        yield return CommonArguments.Help;
    }
}