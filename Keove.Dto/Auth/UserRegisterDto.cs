using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Dto.Auth
{
    public class UserRegisterDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
