using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Response
{
    public class RefreshTokenResponse
    {
        public bool Successfull { get; set; }
        public JWTokenModel Token { get; set; }
    }
}
