using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Response
{
    public class JWTokenModel
    {
        public string AccessToken { get; set; }
        //ticks
        public int ExpiresIn { get; set; }
    }
}
