namespace Fridge.Models;

public class Parameter
{
    public Parameter(Argument argument, string? value)
    {
        Argument = argument;
        Value = value;
    }

    public Argument Argument { get; }
    
    public string? Value { get; }

    public void ThrowIfNullValue()
    {
        if (Value is null)
        {
            throw new ArgumentNullException(Value, $"{Argument.Full} cannot be null.");
        }
    }
}