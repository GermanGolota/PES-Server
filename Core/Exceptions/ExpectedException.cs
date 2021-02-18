using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class ExpectedException : Exception
    {
        public ExpectedException(string message):base(message)
        {

        }
    }
}
