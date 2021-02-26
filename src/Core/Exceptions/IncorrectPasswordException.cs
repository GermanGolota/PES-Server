using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class IncorrectPasswordException : ExpectedException
    {
        public IncorrectPasswordException():base(ExceptionMessages.IncorrectPassword)
        {

        }
    }
}
