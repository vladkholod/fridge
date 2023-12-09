namespace Fridge.Commands.Freeze;

public static class ParserHelper
{
    private const char KeyIdentifier = '"';
    
    public static bool ContainsClosingParenthesis(ReadOnlySpan<char> line)
    {
        return line.Trim().Contains('}');
    }
    
    public  static bool TryGetKey(ReadOnlySpan<char> line, out string key)
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
    
    public static Range GetVersionRange(ReadOnlySpan<char> line)
    {
        var endIndex = line.LastIndexOf(KeyIdentifier);

        var startIndex = line[..endIndex].LastIndexOf(KeyIdentifier);

        return new Range(startIndex + 1, endIndex);
    }
}