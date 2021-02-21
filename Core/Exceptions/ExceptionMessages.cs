using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class ExceptionMessages
    {
        public static string NoChat { get; } = "No such chat";
        public static string NoMessage { get; } = "No such message";
        public static string Unathorized { get; } = "You do not have a permission to do that";
        public static string ServerError { get; } = "Something went wrong";
    }
}
