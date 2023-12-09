namespace Fridge.Extensions;

public static class RangeExtensions
{
    public static int Count(this Range range)
    {
        return range.End.Value - range.Start.Value;
    }
}