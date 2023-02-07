using System;

namespace Core.Exceptions;

public class ExpectedException : Exception
{
    public ExpectedException(string message) : base(message)
    {
    }
}