using Fridge.Models;

namespace Fridge.Commands.Freeze;

public static class FreezeCommandArguments
{
    public static readonly Argument PackageJsonFile = new()
    {
        Full = "--packageFile",
        Short = "-pf",
        Description = "full path to package.json file.",
        RequiresValue = true,
    };

    public static readonly Argument VersionListFile = new()
    {
        Full = "--versionsListFile",
        Short = "-vlf",
        Description = "full path to file with versions of libs (npm list). "
                      + "When not provided installed versions of libs are used.",
        RequiresValue = true,
    };

    public static readonly Argument FreezingDependenciesObject = new()
    {
        Full = "--freezingDependencies",
        Short = "-fd",
        Description = "dependencies to be frozen. "
                      + "Available parameters: dependencies, devDependencies and peerDependencies. "
                      + "All dependencies are included by default.",
        RequiresValue = true,
    };
    
    public static readonly Argument Force = new()
    {
        Full = "--force",
        Short = "-f",
        Description = $"ignores errors if occurs when {VersionListFile.Full} is not specified.",
        RequiresValue = false,
    };

    public static readonly Argument DryRun = new()
    {
        Full = "--dryRun",
        Short = "-dr",
        Description = "does not apply changes but print it out",
        RequiresValue = false,
    };
}