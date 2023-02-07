using System;

namespace Core.Exceptions;

public class NoMessageException : Exception
{
    public NoMessageException() : base(ExceptionMessages.NoMessage)
    {
    }
}