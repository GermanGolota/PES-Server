using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Response
{
    public class JWTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        //unix time-stamp
        public long ExpirationStamp { get; set; }
    }
}
