using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Dto.Auth
{
   public class TokenRequestDto
    {
        public string  Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
