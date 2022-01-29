using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Keove.Dto.Auth
{
    public class UserLoginDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
