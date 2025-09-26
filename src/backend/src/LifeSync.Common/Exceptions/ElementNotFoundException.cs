namespace LifeSync.Common.Exceptions;

public sealed class ElementNotFoundException : Exception
{
    public ElementNotFoundException() { }

    public ElementNotFoundException(string message) : base(message)
    {
    }
}