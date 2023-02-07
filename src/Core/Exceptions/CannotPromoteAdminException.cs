namespace Core.Exceptions;

public class CannotPromoteAdminException : ExpectedException
{
    public CannotPromoteAdminException() : base(ExceptionMessages.CannotPromoteAdmin)
    {
    }
}