using Fridge.Commands.Freeze;
using Fridge.Services;

namespace Fridge;

internal static class Program
{
    public static void Main(string[] args)
    {
        args = new[]
        {
            FreezeCommand.CommandConfig.Name,
            
            FreezeCommandArguments.PackageJsonFile.ToString(),
            "D:\\Work\\projects\\spaceship\\domain-privacy.ui\\package.json",
            
            FreezeCommandArguments.FreezingDependenciesObject.ToString(),
            "peerDependencies",
            
            FreezeCommandArguments.FreezingDependenciesObject.ToString(),
            "dependencies",
            
            FreezeCommandArguments.Force.ToString(),
        };
        
        ICommandRunner commandRunner = new CommandRunner();
        
        commandRunner.Run(args);
    }
}