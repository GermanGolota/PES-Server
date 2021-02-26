using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class NoChatException : ExpectedException
    {
        public NoChatException():base(ExceptionMessages.NoChat)
        {

        }
    }
}
