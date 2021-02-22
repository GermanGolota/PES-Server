namespace Core.Exceptions
{
    public class UnauthorizedException : ExpectedException
    {
        public UnauthorizedException():base(ExceptionMessages.Unathorized)
        {

        }
    }
}
