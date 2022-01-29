using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Dto.Auth
{
   public class TokenResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public List<String> Errors { get; set; }

    }
}
