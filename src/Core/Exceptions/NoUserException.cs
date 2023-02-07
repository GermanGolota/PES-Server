namespace Core.Exceptions;

public class NoUserException : ExpectedException
{
    public NoUserException() : base(ExceptionMessages.NoUser)
    {
    }
}