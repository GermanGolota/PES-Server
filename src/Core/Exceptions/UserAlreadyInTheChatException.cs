namespace Core.Exceptions;

public class UserAlreadyInTheChatException : ExpectedException
{
    public UserAlreadyInTheChatException() : base(ExceptionMessages.UserAlreadyInChat)
    {
    }
}