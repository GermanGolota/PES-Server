﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class ExceptionMessages
    {
        public static string NoChat { get; } = "No such chat";
        public static string NoMessage { get; } = "No such message";
        public static string NoUser { get; } = "No such user";
        public static string Unathorized { get; } = "You do not have a permission to do that";
        public static string ServerError { get; } = "Something went wrong";
        public static string UserAlreadyInChat { get; } = "This user is already present in this chat";
        public static string CannotPromoteAdmin { get; } = "Admin can not be promoted";
    }
}
