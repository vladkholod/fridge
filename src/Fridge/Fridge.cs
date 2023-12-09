using Fridge.Extensions;

namespace Fridge;

public class Fridge
{
    private static readonly string[] FreezeObjects = { "dependencies", "devDependencies", "peerDependencies" };
    
    private const char KeyIdentifier = '"';
    
    private readonly FileInfo _packageFile;
    private readonly FileInfo _versionsFile;

    public Fridge(string packagePath, string npmJsonPath)
    {
        _packageFile = new FileInfo(packagePath);
        if (!_packageFile.Exists)
        {
            throw new FileNotFoundException(_packageFile.FullName);
        }

        _versionsFile = new FileInfo(npmJsonPath);
        if (!_versionsFile.Exists)
        {
            throw new FileNotFoundException(_versionsFile.FullName);
        }
    }

    public void Freeze()
    {
        var libs = GetLibs(_versionsFile);

        var tempFilePath = Path.GetTempFileName();

        var freeze = false;
        using (var writer = new StreamWriter(tempFilePath))
        using (var reader = _packageFile.OpenText())
        {
            while (reader.ReadLine() is { } line)
            {
                if (IsClosingParenthesis(line))
                {
                    freeze = false;
                    writer.WriteLine(line);
                    continue;
                }

                if (TryGetKey(line, out var key) && FreezeObjects.Contains(key))
                {
                    freeze = true;
                    writer.WriteLine(line);
                    continue;
                }

                if (!freeze)
                {
                    writer.WriteLine(line);
                    continue;
                }

                if (!libs.TryGetValue(key, out var version))
                {
                    writer.WriteLine(line);
                    continue;
                }

                var frozenLine = GetFrozenLibLine(line, version);
                writer.WriteLine(frozenLine);
            }
        }

        File.Copy(tempFilePath, _packageFile.FullName, true);
        File.Delete(tempFilePath);
    }

    private static Dictionary<string, string> GetLibs(FileInfo versionsFile)
    {
        using var stream = versionsFile.OpenText();

        var libs = new Dictionary<string, string>();

        while (stream.ReadLine() is { } line)
        {
            if (TryParseVersionLine(line, out var lib))
            {
                libs.TryAdd(lib.Name, lib.Version);
            }
        }

        return libs;
    }

    private static bool TryParseVersionLine(ReadOnlySpan<char> line, out (string Name, string Version) lib)
    {
        const char libNameIdentifier = ' ';
        const char versionSeparator = '@';

        lib = default;

        line = line.Trim();

        var versionSeparatorIndex = line.LastIndexOf(versionSeparator);
        if (versionSeparatorIndex == -1)
        {
            return false;
        }

        var nameIndex = line.IndexOf(libNameIdentifier);
        if (nameIndex == -1)
        {
            return false;
        }

        var libName = line[(nameIndex + 1)..versionSeparatorIndex];

        var version = line[(versionSeparatorIndex + 1)..];

        lib = (libName.ToString(), version.ToString());
        return true;
    }

    private static bool TryGetKey(ReadOnlySpan<char> line, out string key)
    {
        key = string.Empty;

        var startIndex = line.IndexOf(KeyIdentifier);
        if (startIndex == -1)
        {
            return false;
        }

        line = line[(startIndex + 1)..];
        
        var endIndex = line.IndexOf(KeyIdentifier);
        if (endIndex == -1)
        {
            return false;
        }

        key = line[..endIndex].ToString();
        return true;
    }

    private static string GetFrozenLibLine(string origin, string version)
    {
        var versionRange = GetVersionRange(origin);

        var frozenLibLine = origin;

        frozenLibLine = frozenLibLine.Remove(versionRange.Start.Value, versionRange.Count());
        frozenLibLine = frozenLibLine.Insert(versionRange.Start.Value, version);

        return frozenLibLine;

        static Range GetVersionRange(ReadOnlySpan<char> line)
        {
            var endIndex = line.LastIndexOf(KeyIdentifier);

            var startIndex = line[..endIndex].LastIndexOf(KeyIdentifier);

            return new Range(startIndex + 1, endIndex);
        }
    }

    private static bool IsClosingParenthesis(ReadOnlySpan<char> line)
    {
        return line.Trim().Contains('}');
    }
}