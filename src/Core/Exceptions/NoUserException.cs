using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class NoUserException : ExpectedException
    {
        public NoUserException():base(ExceptionMessages.NoUser)
        {

        }
    }
}
