namespace Fridge.Commands.Freeze.LibResolvers;

public interface ILibResolver
{
    Dictionary<string, string> Resolve();
}