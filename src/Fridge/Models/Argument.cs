namespace Fridge.Models;

public class Argument
{
    public string Short { get; init; } = null!;

    public string Full { get; init; } = null!;

    public string Description { get; init; } = null!;

    public bool RequiresValue { get; init; } = false;

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj switch
        {
            string other => Short == other || Full == other,
            Argument other => Short == other.Short && Full == other.Full,
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Short, Full);
    }

    public override string ToString()
    {
        return Full;
    }
}