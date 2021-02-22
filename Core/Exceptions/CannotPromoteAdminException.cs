﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class CannotPromoteAdminException : ExpectedException
    {
        public CannotPromoteAdminException():base(ExceptionMessages.CannotPromoteAdmin)
        {
                
        }
    }
}
