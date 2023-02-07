namespace Core.Exceptions;

public class IncorrectPasswordException : ExpectedException
{
    public IncorrectPasswordException() : base(ExceptionMessages.IncorrectPassword)
    {
    }
}