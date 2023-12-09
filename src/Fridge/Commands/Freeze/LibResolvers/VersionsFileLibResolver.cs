namespace Fridge.Commands.Freeze.LibResolvers;

public class VersionsFileLibResolver : ILibResolver
{
    private readonly FileInfo _versionsListFile;

    public VersionsFileLibResolver(FileInfo versionsListFile)
    {
        if (!versionsListFile.Exists)
        {
            throw new FileNotFoundException(versionsListFile.FullName);
        }
        
        _versionsListFile = versionsListFile;
    }
    
    public Dictionary<string, string> Resolve()
    {
        using var stream = _versionsListFile.OpenText();

        return NpmListHelper.ParseStream(stream);
    }
}