﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class UserToChat
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
    }
}