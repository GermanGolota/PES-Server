namespace Core.Exceptions;

public class NoChatException : ExpectedException
{
    public NoChatException() : base(ExceptionMessages.NoChat)
    {
    }
}