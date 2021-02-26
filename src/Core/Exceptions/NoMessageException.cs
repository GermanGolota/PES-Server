using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class NoMessageException : Exception
    {
        public NoMessageException():base(ExceptionMessages.NoMessage)
        {

        }
    }
}
