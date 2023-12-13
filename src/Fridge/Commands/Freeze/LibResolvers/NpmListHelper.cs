namespace Fridge.Commands.Freeze.LibResolvers;

public static class NpmListHelper
{
    public static Dictionary<string, string> ParseStream(StreamReader streamReader)
    {
        var libs = new Dictionary<string, string>();

        while (streamReader.ReadLine() is { } line)
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
        if (nameIndex == -1 || nameIndex > versionSeparatorIndex)
        {
            return false;
        }

        var libName = line[(nameIndex + 1)..versionSeparatorIndex];

        var version = GetVersion(line[(versionSeparatorIndex + 1)..]);

        lib = (libName.ToString(), version.ToString());
        return true;
    }

    private static ReadOnlySpan<char> GetVersion(ReadOnlySpan<char> line)
    {
        line = line.Trim("^~><");

        var endIndex = 0;
        foreach (var character in line)
        {
            if (!char.IsDigit(character) && !char.IsLetter(character) && !IsAllowedSpecial(character))
            {
                break;
            }

            endIndex++;
        }

        return line[..endIndex];

        static bool IsAllowedSpecial(char character)
        {
            return character == '-' || character == '.';
        }
    }
}