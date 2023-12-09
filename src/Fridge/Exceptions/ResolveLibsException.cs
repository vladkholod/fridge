namespace Fridge.Exceptions;

public class ResolveLibsException : Exception
{
    public ResolveLibsException(Exception innerException)
        : base("Error occured during resolving of libs", innerException)
    {
    }
}